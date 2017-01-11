// 
// 
// 

#include "SensorData.h"

SensorDataClass::SensorDataClass()
{
}

SensorDataClass::SensorDataClass(const char id, float value)
{
	Id = id;
	Value = value;
}

SensorDataClass SensorData;

