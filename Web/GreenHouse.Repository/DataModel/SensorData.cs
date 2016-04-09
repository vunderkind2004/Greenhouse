using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace GreenHouse.Repository.DataModel
{
    public class SensorData : BaseModel
    {
        public DateTime EventDateTime { get; set; }
        public float Value { get; set; }

        [References(typeof(SensorType))]
        public int SensorTypeId { get; set; }
        [Reference]
        public SensorType SensorType { get; set; }

        [References(typeof(Device))]
        public int DeviceId { get; set; }
        [Reference]
        public Device Device { get; set; }
    }
}
