//WiFi temperature an humidity sensor
//Connect DHT sensor to pin D2

#include <ESP8266WiFi.h>
#include "SensitiveSettings.h"
#include "SensorDataModel.h"
#include <DHT.h>

// --- sonsor ---
#define DHTPIN D2     // pin for Data
//select sensor
//#define DHTTYPE DHT11   // DHT 11 
#define DHTTYPE DHT22   // DHT 22  (AM2302)
//#define DHTTYPE DHT21   // DHT 21 (AM2301)
#define ResetOut 10

#define Debag_with_serial 0

//DHT init
DHT dht(DHTPIN, DHTTYPE);

//Def
#define myPeriodic 15 //in sec | Thingspeak pub is 15sec
#define ONE_WIRE_BUS 2  // DS18B20 on arduino pin2 corresponds to D4 on physical board
//#define mySSR 0  // Solid State Relay on pin 0

//OneWire oneWire(ONE_WIRE_BUS);
//DallasTemperature DS18B20(&oneWire);
float prevTemp = 0;

int sent = 0;
void setup() {
	Serial.begin(115200);
	Serial.println("Hello");
	connectWifi();
}

void loop() {
	
	if (WiFi.status() == WL_CONNECTED)
	{
		char *message = CreateSensorsMessage();
		SendMessage(message);
		delete message;
	}
	else
	{
		reconnectWiFi();
	}

	int count = myPeriodic;
	while (count--)
		delay(1000);
}

void SendMessage(char *message)
{
	WiFiClient client;

	if (client.connect(server, port)) { 
		Serial.println("WiFi Client connected ");

		char headerAndData[500];
		strcpy(headerAndData, "Content-Length: ");
		char contentLength[20];
		sprintf(contentLength, "%i", strlen(message));
		strcat(headerAndData, contentLength);
		strcat(headerAndData, "\r\n\r\n");
		strcat(headerAndData, message);
		Serial.println(headerAndData);

		client.print("POST /api/device/AddGreenhouseData HTTP/1.1\n");

		client.print("Host: ");
		client.print(server);
		client.print(":");
		client.print(port);
		client.print("\n");
		
		client.print("Connection: close\n");		
		
		client.print("Content-Type: application/json\n");
		
		Serial.println(headerAndData);
		client.print(headerAndData);
		
		delay(1000);

	}//end if
	sent++;
	client.stop();



	
	//ether.httpPost(webApiAddress, webApiHost, headerAndData, "", api_callback);
}

void reconnectWiFi()
{
	Serial.println("Reconnect WiFi");
	WiFi.reconnect();
	
}

void connectWifi()
{
	Serial.print("Connecting to " + *MY_SSID);
	WiFi.begin(MY_SSID, MY_PWD);
	if (WiFi.status() != WL_CONNECTED) {
		delay(1000);
		Serial.print("conncting ...");
	}

	if (!WiFi.isConnected())
		return;

	Serial.println("");
	Serial.println("Connected");
	Serial.println("");
}//end connect

// -------------  Call to Web API  -----------------
void BeginMessageBuild(char *message)
{
	strcpy(message, "{'DeviceToken':'");
	strcat(message, deviceToken);
	strcat(message, "', 'SensorsData':[");
}

void AddSensorData(char *message, SensorDataModel *sensorData, bool isFirst)
{
	char value[10];
	String((*sensorData).Value).toCharArray(value, 10);
	char id[10];
	String((*sensorData).Id).toCharArray(id, 10);
	if (!isFirst)
		strcat(message, ",");
	strcat(message, "{'SensorId':");
	strcat(message, id);
	strcat(message, ",'Value':");
	strcat(message, value);
	strcat(message, "}");
}

char *CreateSensorsMessage()
{
	char *message;
	message = new char[700];
	BeginMessageBuild(message);

	SensorDataModel *temperatureIn = GetTemperatureIn();
	AddSensorData(message, temperatureIn, true);
	delete temperatureIn;

	SensorDataModel *humidityIn = GetHumidityIn();
	AddSensorData(message, humidityIn, false);
	delete humidityIn;

	EndMessageBuild(message);
	Serial.println(message);

	//delete message;
	return message;
}

void EndMessageBuild(char *message)
{
	strcat(message, "]}");
}


SensorDataModel *GetTemperatureIn()
{
	float t = dht.readTemperature();
	//float t = 20.1 + sent;
	SensorDataModel *data = new SensorDataModel(*temperatureInSensorId, t);
	if (isnan(t))
		Serial.println("Error reading temperature from DHT");
	return data;
}

SensorDataModel *GetHumidityIn()
{
	float h = dht.readHumidity();
	//float h = 50 + sent;
	SensorDataModel *data = new SensorDataModel(*humidityInSensorId, h);
	if (isnan(h))
		Serial.println("Error reading humidity from DHT");
	return data;
}

