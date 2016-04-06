using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Repository.DataModel
{
    public class SensorType : BaseModel
    {
        public string TypeName { get; set; }
        public string Dimension { get; set; }
    }
}
