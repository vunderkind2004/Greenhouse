using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.Responses
{
    public class GetGroupedSensorDataResponse
    {
        public IEnumerable<SensorDataGrouped> GroupedData { get; set; }
        public Dictionary<int, string> SensorIdLabels { get; set; }
    }
}
