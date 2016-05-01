


#include <EtherCard.h>
#include <TimeLib.h>
uint32_t timer;
uint32_t timerInterval;

const char website[] PROGMEM = "www.timeapi.org";
const char requestUrl[] PROGMEM = "http://www.timeapi.org";
const char timeRequest[] = "/gmt/in+2+hours";
const char userAgentHeader[] PROGMEM = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64)";
const char timeDnsFailed[] PROGMEM = "DNS failed for time request";
byte Ethernet::buffer[500]; // tcp/ip send and receive buffer
byte session;
const int dstPort PROGMEM = 80;
static byte mymac[] = { 0x74,0x69,0x69,0x2D,0x30,0x31 };

#define STATIC 0  // set to 1 to disable DHCP (adjust myip/gwip values below)

#if STATIC
// ethernet interface ip address
//static byte myip[] = { 192,168,1,108 };
static byte myip[] = { 192,168,1,108 };
// gateway ip address
static byte gwip[] = { 192,168,1,1 };
#endif

void setup(){
	Serial.begin(57600);
	timerInterval = 5000 * 5;
	if (ether.begin(sizeof Ethernet::buffer, mymac,53) == 0) 
		Serial.println( "Failed to access Ethernet controller");
	#if STATIC
	  ether.staticSetup(myip, gwip,gwip);
	#else
	  if (!ether.dhcpSetup())
		Serial.println("DHCP failed");
	#endif
}

void loop(){

	ether.packetLoop(ether.packetReceive());

	if(millis()>timer)
	{
	  timer+= timerInterval;
	  if(timeStatus()== timeNotSet)
	  {
		  SetTime();
	  }
	  else
	  {

		  DisplayDigitalClock();
	  }
	}
}

void SetTime()
{
	if (!ether.dnsLookup(website))
	{
		Serial.println(timeDnsFailed);
		return;
	}
	ether.browseUrl(requestUrl, timeRequest, website, userAgentHeader, GetTime_callback);
}

static void GetTime_callback (byte status, word off, word len) {
  Serial.println("time received");
  Ethernet::buffer[off+300] = 0;
  char *pResponse = (char*) Ethernet::buffer + off ;
  Serial.println(pResponse); 

  const byte dateTimeLenght = 25;
  char *tempArray = pResponse + len - dateTimeLenght;

  Serial.println("tempArray length");
  Serial.println(len);

  Serial.println(tempArray);

  SetTimeFromString(tempArray);
}

static void SetTimeFromString(char *dateTimeArr)
{
	//2016-05-01T18:15:09+01:00
	Serial.println("SetTimeFromString...");
	Serial.println(dateTimeArr);

	int ACI_delta = 48;
	int year = 0;
	int month = 0;
	int day = 0;
	int hour = 0;
	int min =0;
	int sec = 0;
	byte count=0;
	for (byte i =0; i<19; i++)
	{
		char c = dateTimeArr[i];
		int number  = (int) c - 48;
		//Serial.println(number);
		if(number >= 0 && number<=9)
		{
			if(i>=0 && i<4)
			{
				count++;
				year *= 10;
				year += number;
			}
			if(i>=5 && i<=6)
			{
				count++;
				month *= 10;
				month += number;
			}
			if(i>=8 && i<=9)
			{
				count++;
				day *= 10;
				day += number;
			}
			if(i>=11 && i<=12)
			{
				count++;
				hour *= 10;
				hour += number;
			}
			if(i>=14 && i<=15)
			{
				count++;
				min *= 10;
				min += number;
			}
			if(i>=17 && i<=18)
			{
				count++;
				sec *= 10;
				sec += number;
			}
		}
	}
	if(count != 14)
	{
		Serial.println("wrong date time parsing. count="); 
		Serial.print(count);
		return;
	}
	Serial.println(year);
	Serial.println(month);
	Serial.println(day);
	Serial.println(hour);
	Serial.println(min);
	Serial.println(sec);
	setTime(hour,min,sec,day,month,year);	
	Serial.println("time was set");
}

void DisplayDigitalClock() {
  // digital clock display of the time
  Serial.print(hour());
  printDigits(minute());
  printDigits(second());
  Serial.print(" ");
  Serial.print(dayShortStr(weekday()));
  Serial.print(" ");
  Serial.print(day());
  Serial.print(" ");
  Serial.print(monthShortStr(month()));
  Serial.print(" ");
  Serial.print(year()); 
  Serial.println(); 
}

void printDigits(int digits) {
  // utility function for digital clock display: prints preceding colon and leading 0
  Serial.print(":");
  if(digits < 10)
    Serial.print('0');
  Serial.print(digits);
}

