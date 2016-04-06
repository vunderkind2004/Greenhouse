using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Repository.DataModel
{
    public class SensorData : BaseModel
    {
        public DateTime EventDateTime { get; set; }
        public float Value { get; set; }

        public virtual SensorType SensorType { get; set; }
        public virtual Device Device { get; set; }
    }
}
