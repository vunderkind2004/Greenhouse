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

        [References(typeof(Sensor))]
        public int SensorId { get; set; }
        [Reference]
        public Sensor Sensor { get; set; }
        
    }
}
