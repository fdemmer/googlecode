
// required libraries
#include "XmlRpc.h"		// xmlrpc communication
#include "argtable2.h"	// commandline parser

// collection of methods encapsuling generally required functionality
#include "helpers.h"

// some blabla
#define FG_NAME         "FreevoGenial"
#define FG_DESCRIPTION  "Schedule Freevo recordings remotely."
#define FG_VERSION      "0.2"
#define FG_CONTACT      "Florian Demmer <fdemmer@gmail.com>"

/**
 * \ingroup FreevoGenial
 *
 *
 */
class XmlrpcClient
{
	XmlRpc::XmlRpcClient client;

public:
	/**
	*
	* \param *host address of Freevo machine
	* \param port port where PluginXmlrpc is listening
	* \return 
	*/
	XmlrpcClient(
		const char*	host, 
		int			port
		);
	~XmlrpcClient();

	/**
	* remove a recording
	* \param *channel_id identifier of the channel used in Freevo
	* \param *start time to begin recording
	* \param *title name of the show to record
	* \return 
	*/
	int delRecording(
		const char* channel_id,
		const char* start,
		const char* title
		);

	/**
	* add a recording
	* \param *channel_id identifier of the channel used in Freevo
	* \param *start time to begin recording
	* \param *stop time to end recording (set to NULL to use duration)
	* \param duration time from start to end in minutes (is only used if end = NULL)
	* \param *title name of the show to record
	* \param *descr some show description
	* \return 
	*/
	int addRecording(
		const char* channel_id,
		const char* start,
		const char* stop,
		int			duration,
		const char* title,
		const char* descr
		);

private:
	/**
	* create an XmlRpcValue instance and fill in all required data
	* \param *channel_id identifier of the channel used in Freevo
	* \param *start time to begin recording
	* \param *stop time to end recording (set to NULL to use duration)
	* \param *title name of the show to record
	* \param *sub_title some program description
	* \param *desc some program description
	* \return 
	*/
	XmlRpc::XmlRpcValue getXmlRpcValue(
		const char* channel_id,
		float		start, 
		float		stop, 
		const char* title, 
		const char* sub_title, 
		const char* desc
		);

};



/**
* \ingroup FreevoGenial
*
*/
class FreevoGenial
{
	XmlrpcClient* client;

	int exitcode;
	int nerrors;
	void* argtable[11];
	struct arg_str *argAddress;
	struct arg_str *argAction;
	struct arg_str *argStart;
	struct arg_str *argEnding;
	struct arg_int *argLength;
	struct arg_str *argChannel;
	struct arg_str *argTitle;
	struct arg_str *argDescr;
	struct arg_lit *argHelp;
	struct arg_lit *argVersion;
	struct arg_end *end;

public:
	FreevoGenial(int argc, char* argv[]);
	~FreevoGenial(){};

	/**
	* analyze arguments and actually do stuff
	* \return 
	*/
	int run();

private:
	/**
	 * parse the commandlien using argtable2
	 * \param argc 
	 * \param argv[] 
	 */
	void parseArguments(int argc, char* argv[]);
	/**
	* exit gracefully
	*/
	void bailout();
	/**
	 * exit gracefully
	 * \param msg stderr output
	 */
	void bailout(char* msg);

	/**
	 * add a recording using the argtable information
	 * \return 
	 */
	int rec();
	/**
	 * delete a recording using the argtable information
	 * \return 
	 */
	int del();
	/**
	 * retrieve information about a recording
	 * \return 
	 */
	int get();

};

