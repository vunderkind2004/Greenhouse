#include <DHT.h>
#include "SensorDataModel.h"
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

//--------------config for presure meter: ------------------
#include <Wire.h>
#define BMP085_ADDRESS 0x77  // I2C address of BMP085
//connect 
//VIN to 3.3V
//for Arduino Mega
//SDA to pin 20
//SCL to pin 21
const unsigned char OSS = 0;  // Oversampling Setting
// Calibration values
int ac1;
int ac2;
int ac3;
unsigned int ac4;
unsigned int ac5;
unsigned int ac6;
int b1;
int b2;
int mb;
int mc;
int md;
// b5 is calculated in bmp085GetTemperature(...), this variable is also used in bmp085GetPressure(...)
// so ...Temperature(...) must be called before ...Pressure(...).
long b5;
short temperature;
long pressure;
//------------  end of config for presure meter  ---------------

uint32_t timer;
uint32_t timerInterval;

byte Ethernet::buffer[500]; // tcp/ip send and receive buffer
byte session;
byte ethernetStatus; // 0-?, 1-ok
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
#define DHTPIN 2     // pin for Data
//select sensor
//#define DHTTYPE DHT11   // DHT 11 
#define DHTTYPE DHT22   // DHT 22  (AM2302)
//#define DHTTYPE DHT21   // DHT 21 (AM2301)
#define ResetOut 10

#define Debag_with_serial 0

//DHT init
DHT dht(DHTPIN, DHTTYPE);

void setup(){
	Serial.begin(57600);
	pinMode(ResetOut, OUTPUT);
	digitalWrite(ResetOut, HIGH);
	timerInterval = 1000 * 30;
	dht.begin();
	InitializeEthernet();
	InitializePesureMeter();
	Serial.println("Setup End");

}

void loop(){

	ether.packetLoop(ether.packetReceive());

	if(millis()>timer)
	{
	  timer+= timerInterval;

	  Serial.println("cycle");
	  if(ethernetStatus==0)
	  {
		  Reset();
	  }
	  char *message = CreateSensorsMessage();
	  SendMessage(message);
	  delete message;
	  ethernetStatus = 0;	
	}
}

void Reset()
{
	Serial.println("RESET");
	delay(200);
	digitalWrite(ResetOut, LOW);
	delay(500); 
	digitalWrite(ResetOut, HIGH);
}

void InitializeEthernet()
{
	Serial.println("Initializing Ethernet...");
	
	if (ether.begin(sizeof Ethernet::buffer, mymac,53) == 0) 
	{
		Serial.println( "Failed to access Ethernet controller");
		return;
	}
	#if STATIC
	  ether.staticSetup(myip, gwip,gwip);
	#else
	  if (!ether.dhcpSetup())
	  {
		Serial.println("DHCP failed");
		return;
	  }
	#endif
	ethernetStatus = 1;
	Serial.println("Ethernet initialized");
}

void InitializePesureMeter()
{
	Serial.println("Initialize presuremeter");
	Wire.begin();  
	bmp085Calibration();
}


// -------------  Call to Web API  -----------------

char *CreateSensorsMessage()
{
	char *message;
	message = new char[700];
	BeginMessageBuild(message);

	SensorDataModel *temperatureIn = GetTemperatureIn();	 	  
	AddSensorData(message,temperatureIn,true);
	delete temperatureIn;

	SensorDataModel *humidityIn = GetHumidityIn();	 	  
	AddSensorData(message,humidityIn,false);
	delete humidityIn;

	SensorDataModel *temperatureOut = GetTemperatureOut();	 	  
	AddSensorData(message,temperatureOut,false);
	delete temperatureOut;

	SensorDataModel *presureData = GetPresure();	 	  
	AddSensorData(message,presureData,false);
	delete presureData;

	EndMessageBuild(message);
	Serial.println(message);

	//delete message;
	return message;
}

void SendMessage(char *message)
{
	char *ip;
	ip = new char[50];
	strcpy(ip, webApiIp);
	Serial.println(ip);
	ether.parseIp(ether.hisip,ip);
	delete[] ip;

	ether.printIp("hisip: ", ether.hisip);   

	ether.hisport = webApiPort;
	char headerAndData[500];
	strcpy(headerAndData, "Content-Length: ");
	char contentLength[20];
	sprintf(contentLength,"%i",strlen(message));
	strcat(headerAndData, contentLength);
	strcat(headerAndData, "\r\n\r\n");
	strcat(headerAndData, message);
	Serial.println(headerAndData);
	ether.httpPost(webApiAddress,webApiHost, headerAndData, "", api_callback);		
}

static void api_callback (byte status, word off, word len) {
  Serial.println("azure callback");
  Ethernet::buffer[off+300] = 0;
  char *pResponse = (char*) Ethernet::buffer + off ;
  Serial.println(pResponse); 
  ethernetStatus = 1;
}

void BeginMessageBuild(char *message)
{
	strcpy(message, "{'DeviceToken':'");
	strcat(message, deviceToken);
	strcat(message, "', 'SensorsData':[");
}

void AddSensorData(char *message, SensorDataModel *sensorData, bool isFirst)
{
	char value[10];
	String((*sensorData).Value).toCharArray(value,10);
	char id[10];
	String((*sensorData).Id).toCharArray(id,10);
	if(!isFirst)	
		strcat(message, ",");	
	strcat(message, "{'SensorId':");
	strcat(message, id);
	strcat(message, ",'Value':");	
	strcat(message, value);
	strcat(message, "}");
}

void EndMessageBuild(char *message)
{
	strcat(message, "]}");
}


//----------------- Sensors ---------------------------------------------
SensorDataModel *GetTemperatureIn()
{
    float t = dht.readTemperature();
	SensorDataModel *data = new SensorDataModel(*temperatureInSensorId,t);
	if(isnan(t))
		Serial.println("Error reading temperature from DHT");	
	return data;
}

SensorDataModel *GetHumidityIn()
{
	float h = dht.readHumidity();
	SensorDataModel *data = new SensorDataModel(*humidityInSensorId,h);
	if(isnan(h))
		Serial.println("Error reading humidity from DHT");	
	return data;
}

SensorDataModel *GetTemperatureOut()
{
	temperature = bmp085GetTemperature(bmp085ReadUT());
    float t = temperature/10;
	SensorDataModel *data = new SensorDataModel(*temperatureOutSensorId,t);
	if(isnan(t))
		Serial.println("Error reading temperature out from presuremeter");	
	return data;
}

SensorDataModel *GetPresure()
{
	pressure = bmp085GetPressure(bmp085ReadUP());
    float p = pressure / 133.322368;
	SensorDataModel *data = new SensorDataModel(*presureSensorId,p);
	if(isnan(p))
		Serial.println("Error reading presure");	
	return data;
}

//---------------- Helpers --------------------------------------------

void StringToCharArray(String input, char * output )
{
	int len = input.length();
	output = new char[len];
	input.toCharArray(output, len);
}


// -------------------- PresureMeter Methods ----------------------------

// Stores all of the bmp085's calibration values into global variables
// Calibration values are required to calculate temp and pressure
// This function should be called at the beginning of the program
void bmp085Calibration()
{
  Serial.println("Calibration...");
  ac1 = bmp085ReadInt(0xAA);
  ac2 = bmp085ReadInt(0xAC);
  ac3 = bmp085ReadInt(0xAE);
  ac4 = bmp085ReadInt(0xB0);
  ac5 = bmp085ReadInt(0xB2);
  ac6 = bmp085ReadInt(0xB4);
  b1 = bmp085ReadInt(0xB6);
  b2 = bmp085ReadInt(0xB8);
  mb = bmp085ReadInt(0xBA);
  mc = bmp085ReadInt(0xBC);
  md = bmp085ReadInt(0xBE);
  Serial.println("Calibration is finished.");
}

// Calculate temperature given ut.
// Value returned will be in units of 0.1 deg C
short bmp085GetTemperature(unsigned int ut)
{
  long x1, x2;
 
  x1 = (((long)ut - (long)ac6)*(long)ac5) >> 15;
  x2 = ((long)mc << 11)/(x1 + md);
  b5 = x1 + x2;

  return ((b5 + 8)>>4);  
}

// Calculate pressure given up
// calibration values must be known
// b5 is also required so bmp085GetTemperature(...) must be called first.
// Value returned will be pressure in units of Pa.
long bmp085GetPressure(unsigned long up)
{
  long x1, x2, x3, b3, b6, p;
  unsigned long b4, b7;
 
  b6 = b5 - 4000;
  // Calculate B3
  x1 = (b2 * (b6 * b6)>>12)>>11;
  x2 = (ac2 * b6)>>11;
  x3 = x1 + x2;
  b3 = (((((long)ac1)*4 + x3)<<OSS) + 2)>>2;
 
  // Calculate B4
  x1 = (ac3 * b6)>>13;
  x2 = (b1 * ((b6 * b6)>>12))>>16;
  x3 = ((x1 + x2) + 2)>>2;
  b4 = (ac4 * (unsigned long)(x3 + 32768))>>15;
 
  b7 = ((unsigned long)(up - b3) * (50000>>OSS));
  if (b7 < 0x80000000)
    p = (b7<<1)/b4;
  else
    p = (b7/b4)<<1;
    
  x1 = (p>>8) * (p>>8);
  x1 = (x1 * 3038)>>16;
  x2 = (-7357 * p)>>16;
  p += (x1 + x2 + 3791)>>4;
 
  return p;
}

// Read 1 byte from the BMP085 at 'address'
char bmp085Read(unsigned char address)
{
  unsigned char data;
 
  Wire.beginTransmission(BMP085_ADDRESS);
  Wire.write(address);
  Wire.endTransmission();
 
  Wire.requestFrom(BMP085_ADDRESS, 1);
  while(!Wire.available())
    ;
    
  return Wire.read();
}

// Read 2 bytes from the BMP085
// First byte will be from 'address'
// Second byte will be from 'address'+1
int bmp085ReadInt(unsigned char address)
{
  unsigned char msb, lsb;
 
  Wire.beginTransmission(BMP085_ADDRESS);
  Wire.write(address);
  Wire.endTransmission();
 
  Wire.requestFrom(BMP085_ADDRESS, 2);
  while(Wire.available()<2)
    ;
  msb = Wire.read();
  lsb = Wire.read();
 
  return (int) msb<<8 | lsb;
}

// Read the uncompensated temperature value
unsigned int bmp085ReadUT()
{
  unsigned int ut;
 
  // Write 0x2E into Register 0xF4
  // This requests a temperature reading
  Wire.beginTransmission(BMP085_ADDRESS);
  Wire.write(0xF4);
  Wire.write(0x2E);
  Wire.endTransmission();
 
  // Wait at least 4.5ms
  delay(5);
 
  // Read two bytes from registers 0xF6 and 0xF7
  ut = bmp085ReadInt(0xF6);
  return ut;
}

// Read the uncompensated pressure value
unsigned long bmp085ReadUP()
{
  unsigned char msb, lsb, xlsb;
  unsigned long up = 0;
 
  // Write 0x34+(OSS<<6) into register 0xF4
  // Request a pressure reading w/ oversampling setting
  Wire.beginTransmission(BMP085_ADDRESS);
  Wire.write(0xF4);
  Wire.write(0x34 + (OSS<<6));
  Wire.endTransmission();
 
  // Wait for conversion, delay time dependent on OSS
  delay(2 + (3<<OSS));
 
  // Read register 0xF6 (MSB), 0xF7 (LSB), and 0xF8 (XLSB)
  Wire.beginTransmission(BMP085_ADDRESS);
  Wire.write(0xF6);
  Wire.endTransmission();
  Wire.requestFrom(BMP085_ADDRESS, 3);
 
  // Wait for data to become available
  while(Wire.available() < 3)
    ;
  msb = Wire.read();
  lsb = Wire.read();
  xlsb = Wire.read();
 
  up = (((unsigned long) msb << 16) | ((unsigned long) lsb << 8) | (unsigned long) xlsb) >> (8-OSS);
 
  return up;
}

void PrintPresureAndTemperature()
{
	Serial.println("Get temperature. ");
	temperature = bmp085GetTemperature(bmp085ReadUT());
	Serial.println("Get presure. ");
	pressure = bmp085GetPressure(bmp085ReadUP());
	Serial.print("Temperature: ");
	Serial.print(temperature / 10.0 , 1); 
	Serial.println(" deg C");
	Serial.print("Pressure: ");
	Serial.print(pressure / 133.322368, 0);
	Serial.println(" mmHg");
	Serial.println();
}