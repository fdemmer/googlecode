#include "helpers.h"

std::string stripInvalidChars(const char *text)
{
	std::string ascii;
	for (size_t i=0; i<strlen(text); i++){
		if(text[i] > 0)
			ascii += text[i];
	}
	return ascii;
}

float calcUnixTime(const char *string)
{
	char cYear[] = "0000";
	char cMonth[] = "00";
	char cDay[] = "00";
	char cHour[] = "00";
	char cMinute[] = "00";

	strncpy(cYear,string,4);
	strncpy(cMonth,(string+4),2);
	strncpy(cDay,(string+6),2);
	strncpy(cHour,(string+8),2);
	strncpy(cMinute,(string+10),2);

	struct tm date;
	date.tm_year = atoi(cYear)-1900;
	date.tm_mon = atoi(cMonth)-1;
	date.tm_mday = atoi(cDay);
	date.tm_hour = atoi(cHour);
	date.tm_min = atoi(cMinute);
	date.tm_sec = 0;
	date.tm_isdst = -1;

	return (float)mktime(&date); ;
}