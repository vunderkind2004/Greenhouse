#pragma once
class SensorDataModel
{
	public:
		SensorDataModel();
		SensorDataModel(const char* id, float value);
		const char* Id;
		float Value;
};

extern SensorDataModel SensorData;

