/**The MIT License (MIT)

Copyright (c) 2016 by Rene-Martin Tudyka
Based on the SSD1306 OLED library Copyright (c) 2015 by Daniel Eichhorn (http://blog.squix.ch),
available at https://github.com/squix78/esp8266-oled-ssd1306

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

See more at http://blog.squix.ch
*/
#include <Wire.h>
#include <SPI.h>
#include "SH1106.h"
#include "SH1106Ui.h"
#include "images.h"

// ************ #start region:  temperature WiFi *****************
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

float temperature;
float humidity;
// ************ #end region: temperature WiFi  *****************



// Pin definitions for I2C
//#define OLED_SDA    D2  // pin 14
//#define OLED_SDC    D4  // pin 12
//#define OLED_ADDR   0x3C

/* Hardware Wemos D1 mini SPI pins
 D5 GPIO14   CLK         - D0 pin OLED display
 D6 GPIO12   MISO (DIN)  - not connected
 D7 GPIO13   MOSI (DOUT) - D1 pin OLED display
 D1 GPIO5    RST         - RST pin OLED display
 D2 GPIO4    DC          - DC pin OLED
 D8 GPIO15   CS / SS     - CS pin OLED display
*/

// Pin definitions for SPI
// ESP8266
//#define OLED_RESET  5   // RESET
//#define OLED_DC     4   // Data/Command
//#define OLED_CS     15  // Chip select
// Node MCU
#define OLED_RESET  D1   // RESET
#define OLED_DC     D3   // Data/Command
#define OLED_CS     D8   // Chip select

// Uncomment one of the following based on OLED type
SH1106 display(true, OLED_RESET, OLED_DC, OLED_CS); // FOR SPI
//SH1106   display(OLED_ADDR, OLED_SDA, OLED_SDC);    // For I2C
SH1106Ui ui(&display);

bool msOverlay(SH1106 *display, SH1106UiState* state) {
	display->setTextAlignment(TEXT_ALIGN_RIGHT);
	display->setFont(ArialMT_Plain_10);
	display->drawString(128, 0, String(millis()));
	return true;
}

bool drawFrame1(SH1106 *display, SH1106UiState* state, int x, int y) {
	// draw an xbm image.
	// Please note that everything that should be transitioned
	// needs to be drawn relative to x and y

	// if this frame need to be refreshed at the targetFPS you need to
	// return true
	display->drawXbm(x + 34, y + 14, WiFi_Logo_width, WiFi_Logo_height, WiFi_Logo_bits);
	return false;
}

bool drawFrame2(SH1106 *display, SH1106UiState* state, int x, int y) {
	// Demonstrates the 3 included default sizes. The fonts come from SH1106Fonts.h file
	// Besides the default fonts there will be a program to convert TrueType fonts into this format
	display->setTextAlignment(TEXT_ALIGN_LEFT);
	//display->setFont(ArialMT_Plain_10);
	//display->drawString(0 + x, 10 + y, "Arial 10");

	display->setFont(ArialMT_Plain_16);
	//display->drawString(0 + x, 20 + y, "Arial 16");
	display->drawString(20 + x, 10 + y, "Temp:");
	if (!isnan(temperature))
	{
		display->drawString(70 + x, 10 + y, String(temperature));
	}
	else
	{
		display->drawString(70 + x, 10 + y, "...");
	}

	display->setFont(ArialMT_Plain_16);
	//display->drawString(0 + x, 34 + y, "Arial 24");
	display->drawString(20 + x, 34 + y, "Hum:");
	if (!isnan(humidity))
	{
		display->drawString(70 + x, 34 + y, String(humidity));
	}
	else
	{
		display->drawString(70 + x, 34 + y, "...");
	}

	return false;
}

bool drawFrame3(SH1106 *display, SH1106UiState* state, int x, int y) {
	// Text alignment demo
	display->setFont(ArialMT_Plain_24);	

	// The coordinates define the center of the text
	display->setTextAlignment(TEXT_ALIGN_LEFT);
	if (!isnan(temperature))
	{
		display->drawString(20 + x, 10 + y, String(temperature));
	}
	else
	{
		display->drawString(20 + x, 10 + y, "...");
	}

	display->drawString(80 + x, 10 + y, " C");

	if (!isnan(humidity))
	{
		display->drawString(20 + x, 34 + y, String(humidity));
	}
	else
	{
		display->drawString(20 + x, 34 + y, "...");
	}

	display->drawString(80 + x, 34 + y, " %");

	

	
	return false;
}

//bool drawFrame4(SH1106 *display, SH1106UiState* state, int x, int y) {
//	// Demo for drawStringMaxWidth:
//	// with the third parameter you can define the width after which words will be wrapped.
//	// Currently only spaces and "-" are allowed for wrapping
//	display->setTextAlignment(TEXT_ALIGN_LEFT);
//	display->setFont(ArialMT_Plain_10);
//	display->drawStringMaxWidth(0 + x, 10 + y, 128, "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore.");
//	return false;
//}

// how many frames are there?
int frameCount = 2;
// this array keeps function pointers to all frames
// frames are the single views that slide from right to left
bool(*frames[])(SH1106 *display, SH1106UiState* state, int x, int y) = { drawFrame3, drawFrame2 };

bool(*overlays[])(SH1106 *display, SH1106UiState* state) = { msOverlay };
int overlaysCount = 1;

void setup() {
	Serial.begin(115200);
	Serial.println("Hello");
	connectWifi();


	//ui.setTargetFPS(30);
	ui.setTargetFPS(30);

	ui.setActiveSymbole(activeSymbole);
	ui.setInactiveSymbole(inactiveSymbole);

	// You can change this to
	// TOP, LEFT, BOTTOM, RIGHT
	ui.setIndicatorPosition(BOTTOM);

	// Defines where the first frame is located in the bar.
	ui.setIndicatorDirection(LEFT_RIGHT);

	// You can change the transition that is used
	// SLIDE_LEFT, SLIDE_RIGHT, SLIDE_TOP, SLIDE_DOWN
	ui.setFrameAnimation(SLIDE_LEFT);

	// Add frames
	ui.setFrames(frames, frameCount);

	// Add overlays
	//ui.setOverlays(overlays, overlaysCount);

	// Inital UI takes care of initalising the display too.
	ui.init();

	display.flipScreenVertically();

}

unsigned long lastTime = 0;
unsigned long lastUiTime = 0;
//int uiTimeToUpdate = ;

void loop() {
	unsigned long uiTime = millis();
	if (uiTime > lastUiTime + 0)
	{
		lastUiTime = uiTime;
		int remainingTimeBudget = ui.update();
		//Serial.print("remainingTimeBudget = ");
		//Serial.println(remainingTimeBudget);

		if (remainingTimeBudget > 0) {
			// You can do some work here
			// Don't do stuff if you are below your
			// time budget.
			unsigned long time = millis();
			if (time > lastTime + 1000 * myPeriodic)
			{
				lastTime = time;
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
			}

			/*int count = myPeriodic;
			while (count--)
				delay(1000);*/

			//delay(remainingTimeBudget);
		}
	}
	
}


//*********** #start region: Temperature WiFi methods ************************

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
	SensorDataModel *data = new SensorDataModel(temperatureInSensorId, t);
	if (isnan(t))
		Serial.println("Error reading temperature from DHT");
	temperature = t;
	return data;
}

SensorDataModel *GetHumidityIn()
{
	//float h = dht.readHumidity();
	// temporary humiodity correction for bad sensor
	float h = dht.readHumidity() - 12;
	//float h = 50 + sent;
	SensorDataModel *data = new SensorDataModel(humidityInSensorId, h);
	if (isnan(h))
		Serial.println("Error reading humidity from DHT");
	humidity = h;
	return data;
}

//***********   #end region: Temperature WiFi methods ************************
