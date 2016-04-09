using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.ApiModels
{
    public class SensorDataMessage
    {
        public string Token { get; set; }
        public int SensorType { get; set; }
        public float Value { get; set; }
        //public DateTime? HardwareTime { get; set; }
    }
}
