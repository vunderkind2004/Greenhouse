//*********************************
// connect WiFi module:
// VCV & CH_PD to 3.3 V
// GND to GND
// TX to 0(RX)
// RX to 1(TX)
//*********************************



//#include <JsonObjectBase.h>
//#include <JsonHashTable.h>
//#include <JsonArray.h>
//#include <JsonParser.h>
#include <ArduinoJson.h>
#include <SoftwareSerial.h>
 
#define cs   10  // Выводы на дисплей
#define dc   9
#define rst  8
 
//#include <Adafruit_GFX.h>    // Графическая библиотека
//#include <Adafruit_ST7735.h> // Аппаратная библиотека
#include <SPI.h>
 
//Adafruit_ST7735 tft = Adafruit_ST7735(cs, dc, rst);
 
//using namespace ArduinoJson::Parser;
 
#define SSID "HomeFastInet" // введите ваш SSID
#define PASS "xxxxxxxxx" // введите ваш пароль
#define LOCATIONID "2925533" // id местоположения
#define DST_IP "50.16.215.101" //api.openweathermap.org
//SoftwareSerial Serial(2, 3); // RX, TX для отладки
//JsonParser<32> parser;
StaticJsonBuffer<200> jsonBuffer;

void setup()
{
  Serial3.begin(115200);
//	Serial3.begin(9600);
  Serial3.setTimeout(5000);
  Serial.begin(9600); // для отладки
  Serial.println("Init");
  //tft.initR(INITR_BLACKTAB);
  //tft.setRotation(1);
  //tft.fillScreen(ST7735_BLACK);
  //tft.setCursor(2, 2);
  //tft.setTextColor(ST7735_WHITE);

  Serial3.println("AT+RST");
  delay(200);
  String responce = Serial3.readString();
  Serial.print(responce);

  //Serial3.println("AT+RST"); // сброс и проверка, если модуль готов
  //delay(5000);
  //if(Serial3.find("ready")) {
  //  Serial.println("WiFi - Module is ready");
  //  //tft.println("WiFi - Module is ready");
  //}else{
	 // //String responce = Serial3.readString();
	 // //Serial.print(responce);
  //  Serial.println("Module dosn't respond.");
  //  //tft.println("Module dosn't respond.");
  //  //tft.println("Please reset.");
  //  while(1);
  //}
  delay(1000);
  // try to connect to wifi
  boolean connected=false;
  for(int i=0;i<5;i++){
    if(connectWiFi()){
      connected = true;
      //tft.println("Connected to WiFi...");
      break;
    }
  }
  if (!connected){
    //tft.println("Coudn't connect to WiFi.");
    while(1);
  }
  delay(5000);
  Serial3.println("AT+CIPMUX=0"); // установка в режим одиночного соединения
}
void loop()
{
  String cmd = "AT+CIPSTART=\"TCP\",\"";
  cmd += DST_IP;
  cmd += "\",80";
  Serial3.println(cmd);
  Serial.println(cmd);
  String response = Serial3.readString();
  Serial.println(response);
  if(Serial3.find("Error")) return;
  cmd = "GET http://www.timeapi.org/utc/now.json";
  //cmd += LOCATIONID;
  cmd += " HTTP/1.0";
  cmd+="\r\nUser-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.112 Safari/537.36";
  cmd += "\r\nAccept-Encoding: gzip, deflate, sdch";
  Serial3.print("AT+CIPSEND=");
  Serial3.println(cmd.length());

  String response2 = Serial3.readString();
  Serial.println(response2);

  if(Serial3.find(">")){
    Serial.print(">");
  }else{
    Serial3.println("AT+CIPCLOSE");
    Serial.println("connection timeout");
    //tft.fillScreen(ST7735_BLACK);
    //tft.setCursor(2, 2);
    //tft.setTextColor(ST7735_WHITE);
    //tft.println("connection timeout");
    delay(1000);
    return;
  }
  Serial3.print(cmd);
  Serial.print(cmd);
  //unsigned int i = 0; //счетчик времени
  //int n = 1; // счетчик символов
  //char json[100]="{";
  //while (!Serial.find("\"dateString\"")){}
  //while (i<60000) {
  //  if(Serial3.available()) {
  //    char c = Serial.read();
  //    json[n]=c;
  //    if(c=='}') break;
  //    n++;
  //    i=0;
  //  }
  //  i++;
  //}
  String jsonString = Serial3.readString();
  Serial.println(jsonString);
  char* json;;
  jsonString.toCharArray(json, jsonString.length());
  Serial.println(json);
  JsonObject& root = jsonBuffer.parseObject(json);
  char date = root["dateString"];
  //double pressure = root["pressure"];
  //double humidity = root["humidity"];
  //temp -= 273.15; // Перевод градусов кельвины-цельсии
  //tft.fillScreen(ST7735_BLACK);
  //tft.setCursor(2, 25);
  //tft.setTextColor(ST7735_BLUE);
  //tft.setTextSize(2);
  //tft.print("Temp: ");
  //tft.print((int)temp);
  //tft.print(".");
  //tft.print((int)((temp-(int)temp)*10));
  //tft.println(" C");
  //tft.setCursor(2, 55);
  //tft.setTextColor(ST7735_GREEN);
  //tft.setTextSize(2);
  //tft.print("Press: ");
  //tft.print((int)pressure);
  //tft.setCursor(2, 85);
  //tft.setTextColor(ST7735_YELLOW);
  //tft.setTextSize(2);
  //tft.print("Humidity: ");
  //tft.print((int)humidity);
  //tft.println("%");
  Serial.println(date);
  //Serial.println(pressure);
  //Serial.println(humidity);
  Serial.println("====");
  delay(600000);
}
 
boolean connectWiFi()
{
  Serial3.println("AT+CWMODE=1");
  String cmd="AT+CWJAP=\"";
  cmd+=SSID;
  cmd+="\",\"";
  cmd+=PASS;
  cmd+="\"";
  Serial.println(cmd);
  Serial3.println(cmd);
  delay(2000);
  if(Serial3.find("OK")){
    Serial.println("OK, Connected to WiFi.");
    return true;
  }else{
    Serial.println("Can not connect to the WiFi.");
    return false;
  }
}