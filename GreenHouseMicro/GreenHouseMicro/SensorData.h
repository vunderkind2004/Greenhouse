// SensorData.h

#ifndef _SENSORDATA_h
#define _SENSORDATA_h

#if defined(ARDUINO) && ARDUINO >= 100
	#include "arduino.h"
#else
	#include "WProgram.h"
#endif

class SensorDataClass
{		
  public:	
	SensorDataClass();
	SensorDataClass(float h, float t);
	float HumidityIn;
	float TemperatureIn;
};

extern SensorDataClass SensorData;

#endif

