// Present a "Will be back soon web page", as stand-in webserver.
// 2011-01-30 <jc@wippler.nl> http://opensource.org/licenses/mit-license.php
 
#include <EtherCard.h>

#define STATIC 1  // set to 1 to disable DHCP (adjust myip/gwip values below)

#if STATIC
// ethernet interface ip address
//static byte myip[] = { 192,168,1,108 };
static byte myip[] = { 192,168,1,108 };
// gateway ip address
static byte gwip[] = { 192,168,1,1 };
#endif

// ethernet mac address - must be unique on your network
static byte mymac[] = { 0x74,0x69,0x69,0x2D,0x30,0x31 };
//const char website[] PROGMEM = "192.168.1.106";
//const char website[] PROGMEM = "www.google.com";

const char website[] PROGMEM = "www.timeapi.org";
const char requestUrl[] PROGMEM = "http://www.timeapi.org";
const char timeRequest[] = "/utc/now";
const char userAgentHeader[] PROGMEM = "User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64)";

byte Ethernet::buffer[500]; // tcp/ip send and receive buffer

uint32_t timer;
byte session;
uint8_t *ip;

const int dstPort PROGMEM = 80;

const int srcPort PROGMEM = 80;

const char page[] PROGMEM =
"HTTP/1.0 503 Service Unavailable\r\n"
"Content-Type: text/html\r\n"
"Retry-After: 600\r\n"
"\r\n"
"<html>"
  "<head><title>"
    "Service Temporarily Unavailable"
  "</title></head>"
  "<body>"
    "<h3>This service is currently unavailable</h3>"
    "<p><em>"
      "The main server is currently off-line.<br />"
      "Please try again later."
    "</em></p>"
  "</body>"
"</html>"
;

// called when the client request is complete
static void my_callback (byte status, word off, word len) {
  Serial.println(">>>");
  Ethernet::buffer[off+300] = 0;
  Serial.print((const char*) Ethernet::buffer + off);
  Serial.println("...");
}

static void pingCallBack(uint8_t *)
{
	Serial.println("Ping callback");	
}

void setup(){
  Serial.begin(57600);
  Serial.println("\n[backSoon]");
  
  if (ether.begin(sizeof Ethernet::buffer, mymac,53) == 0) 
    Serial.println( "Failed to access Ethernet controller");
#if STATIC
  ether.staticSetup(myip, gwip,gwip);
#else
  if (!ether.dhcpSetup())
    Serial.println("DHCP failed");
#endif

  //ether.parseIp(ether.hisip,"192.168.1.106");
  //ether.parseIp(ether.netmask, "255.255.255.0");

  Serial.print("server is at ");
  //Serial.println(Ethernet.localIP());

  ether.printIp("IP:  ", ether.myip);
  ether.printIp("GW:  ", ether.gwip);  
  ether.printIp("DNS: ", ether.dnsip);  
  ether.printIp("mask: ", ether.netmask); 
}

void loop(){
  // wait for an incoming TCP packet, but ignore its contents
  /*if (ether.packetLoop(ether.packetReceive())) {
    memcpy_P(ether.tcpOffset(), page, sizeof page);
    ether.httpServerReply(sizeof page - 1);
  }*/

	ether.packetLoop(ether.packetReceive());

  if(millis()>timer)
  {
	  timer+= 5000;
	  Serial.println("time for request");

	  if (!ether.dnsLookup(website))
		Serial.println("DNS failed");
	   
	  ether.printIp("hisip: ", ether.hisip);   

	  //ether.browseUrl(PSTR("www.google.com"), "/", website, my_callback);

	 ether.browseUrl(requestUrl, timeRequest, website, userAgentHeader, my_callback);
  }

  

}