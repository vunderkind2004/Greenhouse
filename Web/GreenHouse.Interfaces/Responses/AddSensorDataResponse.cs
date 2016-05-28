using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.Responses
{
    public class AddSensorDataResponse
    {
        public string UserName { get; set; }
        public IEnumerable<SensorDataResponse> SensorDataResponse { get; set; }
    }
}
