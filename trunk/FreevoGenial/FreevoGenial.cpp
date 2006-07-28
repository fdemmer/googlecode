/**
* \ingroup FreevoGenial2
*
* \version 0.2
* the second encounter :)
*
* \date 09-17-2005
*
* \author Florian Demmer <fdemmer@gmail.com> http://demmer.ipax.at
*
* \par license
* GPL; google for it...
* 
*
*/

#include "FreevoGenial.h"

using namespace std;

// FREEVOGENIAL METHODS

FreevoGenial::FreevoGenial(int argc, char* argv[])
{
	nerrors=0;
	exitcode=0;
	parseArguments(argc, argv);

	string address(argAddress->sval[0]);
	size_t colon = address.find(":");
	string host = address.substr(0, colon);
	int port = atoi(address.substr(colon+1, address.size()).c_str());
	client = new XmlrpcClient(host.c_str(), port);
}

int FreevoGenial::run()
{
	if(!strcmp("rec",argAction->sval[0]))
	{
		return rec();
	}
	if(!strcmp("del",argAction->sval[0]))
	{
		return del();
	}
	if(!strcmp("get",argAction->sval[0]))
	{
		return get();
	}	
	return 1;
}

int FreevoGenial::rec()
{
	if(strlen(argStart->sval[0]) != 12)
		bailout("beginning timestamp invalid!");

	if(argLength->count && argEnding->count)
		bailout("please specify duration or ending only!");

	if(argEnding->count)
	{
		if(strlen(argEnding->sval[0]) != 12)
			bailout("ending timestamp invalid!");

		return client->addRecording(
			argChannel->sval[0],// channel id
			argStart->sval[0],	// start
			argEnding->sval[0],	// end
			0,					// length
			argTitle->sval[0],	// title
			argDescr->sval[0]	// description
			);
	}

	if(argLength->count)
	{
		if(argLength->ival[0] <= 0)
			bailout("length value invalid!");

		return client->addRecording(
			argChannel->sval[0],// channel id
			argStart->sval[0],	// start
			NULL,				// end
			argLength->ival[0],	// length
			argTitle->sval[0],	// title
			argDescr->sval[0]	// description
			);
	}

	return 1;
}

int FreevoGenial::del()
{
	return client->delRecording(
		argChannel->sval[0],// channel id
		argStart->sval[0],	// start
		argTitle->sval[0]	// title
		);
}

int FreevoGenial::get()
{
	//TODO ... well, do it.
	return 1;
}

void FreevoGenial::parseArguments(int argc, char* argv[])
{
	/* Define the allowable command line options, collecting them in argtable[] */
	argAddress	= arg_str1("x", "xmlrpc",	"address:port",   "  ... host and port of Freevo box");
	argAction   = arg_str0("a", "action",	"{rec|del}",   "     ... action to perform (default: rec)");
	argStart    = arg_str1("s", "start",   "YYYYMMDDhhmm",   "   ... beginnig timestamp");
	argEnding   = arg_str0("e", "ending",	"YYYYMMDDhhmm",   "  ... ending timestamp");
	argLength   = arg_int0("l", "length",   "minutes",   "       ... use instead of ending timestamp");
	argChannel  = arg_str1("c", "channel",   "channel_id",   "   ... as set in Freevo configuration");
	argTitle    = arg_str1("t", "title",   "name",   "           ... name of program to record");
	argDescr    = arg_str0("d", "description",   "text",   "     ... short description of program");
	argHelp     = arg_lit0("h", "help",        "                 ... print this help and exit");
	argVersion  = arg_lit0("v", "version",        "              ... print version information and exit");
	end         = arg_end(20);
	argtable[0] = argAddress;	argtable[1] = argAction;
	argtable[2] = argStart;		argtable[3] = argEnding;	
	argtable[4] = argLength;	argtable[5] = argChannel;
	argtable[6] = argTitle;		argtable[7] = argDescr;
	argtable[8] = argHelp;		argtable[9] = argVersion;
	argtable[10] = end;

	/* verify the argtable[] entries were allocated sucessfully */
	if (arg_nullcheck(argtable) != 0){
		/* NULL entries were detected, some allocations must have failed */
		printf("%s: insufficient memory.\n",FG_NAME);
		exitcode=1;
		bailout();
	}

	/* set defaults */
	argAction->sval[0] = "rec";
	argDescr->sval[0] = "no description available";

	/* Parse the command line as defined by argtable[] */
	nerrors = arg_parse(argc,argv,argtable);

	/* special case: '--help' takes precedence over error reporting */
	if (argHelp->count > 0){
		std::cout << FG_NAME << ", Version " << FG_VERSION << " - " << FG_DESCRIPTION << std::endl;
		std::cout << "Usage: " << FG_NAME << " ";
		arg_print_syntax(stdout,argtable,"\n");
		arg_print_glossary(stdout,argtable,"  %-10s %s\n");
		std::cout << std::endl << "Report bugs to " << FG_CONTACT << std::endl;
		bailout();
	}

	/* special case: '--version' takes precedence error reporting */
	if (argVersion->count > 0){
		std::cout << FG_NAME << ", Version " << FG_VERSION << " - " << FG_DESCRIPTION << std::endl;
		std::cout << "Report bugs to " << FG_CONTACT << std::endl;
		bailout();
	}

	/* If the parser returned any errors then display them and exit */
	if (nerrors > 0){
		/* Display the error details contained in the arg_end struct.*/
		arg_print_errors(stdout,end,FG_NAME);
		/* ending or duration is only required for action=rec */
		if(argEnding->count == 0 && argLength->count == 0 && !strcmp("rec",argAction->sval[0]))
		{
			std::cout << FG_NAME << ": missing option -e|--ending=YYYYMMDDhhmm or"  << std::endl;
			std::cout << FG_NAME << ": missing option -l|--length=minutes"  << std::endl;
		}
		std::cout << "Try '" << FG_NAME << " --help' for more information." << std::endl;
		exitcode=1;
		bailout();
	}
}

void FreevoGenial::bailout()
{
	/* deallocate each non-null entry in argtable[] */
	arg_freetable(argtable,sizeof(argtable)/sizeof(argtable[0]));
	exit(exitcode);
}

void FreevoGenial::bailout(char* msg)
{
	/* deallocate each non-null entry in argtable[] */
	arg_freetable(argtable,sizeof(argtable)/sizeof(argtable[0]));
	std::cerr << "Error: " << msg << std::endl;
	exit(exitcode);
}

// XMLRPCCLIENT METHODS

XmlrpcClient::XmlrpcClient(const char* host, int port) : client(host, port) {}
XmlrpcClient::~XmlrpcClient(){}

XmlRpc::XmlRpcValue XmlrpcClient::getXmlRpcValue(const char *channel_id,
												 float start, 
												 float stop,
												 const char *title, 
												 const char *sub_title, 
												 const char *desc)
{
	XmlRpc::XmlRpcValue value;
	value["channel_id"] = stripInvalidChars(channel_id);
	value["title"] = stripInvalidChars(title);
	value["desc"] = stripInvalidChars(desc);
	value["sub_title"] = stripInvalidChars(sub_title);
	value["start"] = start;
	value["stop"] = stop;
	return value;
}

int XmlrpcClient::addRecording(const char *channel_id, const char *start, const char *ending,
							   int duration, const char *title, const char *descr)
{
	XmlRpc::XmlRpcValue prog, result;

	float fStart = calcUnixTime(start);
	float fEnd = 0.0f;

	if(ending != NULL)
		fEnd = calcUnixTime(ending);
	else
		fEnd = fStart+(float)duration*60;

	prog = getXmlRpcValue(channel_id,fStart,fEnd,title,"",descr);

	if (client.execute("scheduleRecording", prog, result))
		return 0;

	return 1;
}

int XmlrpcClient::delRecording(const char *channel_id, const char *start, const char *title)
{
	XmlRpc::XmlRpcValue prog, result;

	prog = getXmlRpcValue(channel_id,calcUnixTime(start),0.0,title,"","");
	if (client.execute("removeScheduledRecording", prog, result))
		return 0;

	return 1;
}

// MAIN METHOD

int main(int argc, char* argv[]){
	FreevoGenial* fg = new FreevoGenial(argc, argv);
	return fg->run();
}


