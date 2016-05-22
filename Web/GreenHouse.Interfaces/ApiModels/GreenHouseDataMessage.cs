using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.ApiModels
{
    public class GreenHouseDataMessage
    {
        public string DeviceToken { get; set; }
        public SensorData[] SensorsData { get; set; }
    }

    public class SensorData
    {
        public int SensorId { get; set; }
        public float Value { get; set; }
    }
}
