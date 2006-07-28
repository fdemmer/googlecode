
#include <iostream>
#include <time.h>

/**
* remove invalid characters
* \param *text 
* \return 
*/
std::string stripInvalidChars(const char *text);

/**
* calculate unix time from a string
* \param *string date and time in the format YYYYMMDDhhmm
* \return unix time stamp
*/
float calcUnixTime(const char *string);