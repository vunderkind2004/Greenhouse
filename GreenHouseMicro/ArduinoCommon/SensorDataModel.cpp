#include "SensorDataModel.h"



SensorDataModel::SensorDataModel()
{
}

SensorDataModel::SensorDataModel(const char* id, float value)
{
	Id = id;
	Value = value;
}

SensorDataModel SensorData;
