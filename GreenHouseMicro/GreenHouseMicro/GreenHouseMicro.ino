#include <DHT.h>
#include "SensorData.h"
#include <EtherCard.h>
#include "device_config.h"

//http://localhost:2898/api/device/addgreenhousedata
//application/json
//Example of POST request body to API:

//{
//	'DeviceToken': '5c87849c-ec57-4b8b-a000-b87aae1f8209',
//	'SensorsData' :
//	[
//		{
//		'SensorId' : 2,
//		'Value' : 31
//		},
//		{
//        'SensorId' : 3,
//        'Value' : 50.5
//        }
//    ]
//}

uint32_t timer;
uint32_t timerInterval;

byte Ethernet::buffer[500]; // tcp/ip send and receive buffer
byte session;
static byte mymac[] = { 0x74,0x69,0x69,0x2D,0x30,0x31 };

#define STATIC 0  // set to 1 to disable DHCP (adjust myip/gwip values below)

#define TEST_Sign 1

#if STATIC
// ethernet interface ip address
//static byte myip[] = { 192,168,1,108 };
static byte myip[] = { 192,168,1,108 };
// gateway ip address
static byte gwip[] = { 192,168,1,1 };
#endif

// --- sonsor ---
#define DHTPIN 2     // к какому пину будет подключен вывод Data
//выбор используемого датчика
//#define DHTTYPE DHT11   // DHT 11 
#define DHTTYPE DHT22   // DHT 22  (AM2302)
//#define DHTTYPE DHT21   // DHT 21 (AM2301)

#define Debag_with_serial 0

//инициализация датчика
DHT dht(DHTPIN, DHTTYPE);

void setup(){
	Serial.begin(57600);
	timerInterval = 1000 * 30;
	dht.begin();
	if (ether.begin(sizeof Ethernet::buffer, mymac,53) == 0) 
		Serial.println( "Failed to access Ethernet controller");
	#if STATIC
	  ether.staticSetup(myip, gwip,gwip);
	#else
	  if (!ether.dhcpSetup())
		Serial.println("DHCP failed");
	#endif
	Serial.println("Setup End");
}

void loop(){

	ether.packetLoop(ether.packetReceive());

	if(millis()>timer)
	{
	  timer+= timerInterval;

	  Serial.println("cycle");

	  SensorDataClass *sensorData = GetSensorData();	  
	  SendToWebApi(sensorData);
	  delete sensorData;
	}
}
// -------------  Call to Web API  -----------------

void SendToWebApi(SensorDataClass *sensorData)
{
	/*if (!ether.dnsLookup(website))
	{
		Serial.println("DnsFailed");
		return;
	}*/
	char humidity[10];
	char temperature[10];	
	String((*sensorData).HumidityIn).toCharArray(humidity,10);
	String((*sensorData).TemperatureIn).toCharArray(temperature,10);


	//const char dataInit[] = "                                                            ";

	char *data;
	data = new char[700];
	strcpy(data, "{'DeviceToken':'");
	strcat(data, deviceToken);
	strcat(data, "', 'SensorsData':[{'SensorId':");
	strcat(data, temperatureInSensorId);
	strcat(data, ",'Value':");	
	strcat(data, temperature);
	strcat(data, "},{'SensorId':");
	strcat(data, humidityInSensorId);
	strcat(data, ",'Value':");	
	strcat(data, humidity);
	strcat(data, "}]}");

	Serial.println(data);
	
	char *ip;
	ip = new char[50];
	strcpy(ip, webApiIp);
	Serial.println(ip);
	ether.parseIp(ether.hisip,ip);
	delete[] ip;

	ether.printIp("hisip: ", ether.hisip);   

	ether.hisport = webApiPort;
	char headerAndData[500];
	//char *header = new char[200];
	//strcpy(header, "Accept: application/json\nContent-Type: application/json");	
	//strcpy(header, "Accept: application/json");	
	strcpy(headerAndData, "Content-Length: ");
	char contentLength[20];
	sprintf(contentLength,"%i",strlen(data));
	strcat(headerAndData, contentLength);
	strcat(headerAndData, "\r\n\r\n");
	strcat(headerAndData, data);
	Serial.println(headerAndData);

	//char header3[] PROGMEM = "Accept: application/json\nContent-Type: application/json\nContent-Length: 200 ";
	//const char header3 = ;

	//const char outData[] = "test";
	
	//char header2[200];
	//strcpy(header2, header);	
	//ether.httpPost(webApiAddress,webApiHost, header, data, api_callback);
	//const char *data2 = "test 123";
	ether.httpPost(webApiAddress,webApiHost, headerAndData, "", api_callback);
	//ether.browseUrl(webApiAddress, "", webApiHost, "", api_callback);

	delete[] data;
	//delete[] data2;
	//delete[] header;
}


static void api_callback (byte status, word off, word len) {
  Serial.println("azure callback");
  Ethernet::buffer[off+300] = 0;
  char *pResponse = (char*) Ethernet::buffer + off ;
  Serial.println(pResponse); 
}


//----------------- Sensors ---------------------------------------------
SensorDataClass *GetSensorData()
{	
	float h = dht.readHumidity();
    float t = dht.readTemperature();

	SensorDataClass *data = new SensorDataClass(h,t);
	//data.HumidityIn = h;
	//data.TemperatureIn = t;

  // проверяем правильные ли данные получили
  if (isnan(t) || isnan(h)) {
    Serial.println("Error reading from DHT");	
  } else {
    Serial.print("H: "); 
    Serial.print(h);
    Serial.print("%  ");
    Serial.print("T: "); 
    Serial.print(t);
    Serial.println("*C");
  }
  return data;
}

//---------------- Helpers --------------------------------------------

void StringToCharArray(String input, char * output )
{
	int len = input.length();
	output = new char[len];
	input.toCharArray(output, len);
}