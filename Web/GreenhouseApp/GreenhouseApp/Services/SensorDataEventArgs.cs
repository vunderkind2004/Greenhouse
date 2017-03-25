using GreenhouseApp.Models;
using System.Collections.Generic;

namespace GreenhouseApp.Services
{
    public class SensorDataEventArgs
    {
        public Dictionary<string, Dictionary<int, UserSensorWithData>> UserSensors { get; private set; }
        public SensorDataEventArgs(Dictionary<string, Dictionary<int, UserSensorWithData>> userSensors)
        {
            UserSensors = userSensors;
        }
    }
}