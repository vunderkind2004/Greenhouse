#pragma once
class SensorDataModel
{
	public:
		SensorDataModel();
		SensorDataModel(const char id, float value);
		char Id;
		float Value;
};

extern SensorDataModel SensorData;

