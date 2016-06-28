using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenHouse.Interfaces.Enums;

namespace GreenHouse.Interfaces.Requests
{
    public class GetGroupedSensorDataRequest
    {
        public string UserName {get;set;}
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public GroupByTime GroupByTime { get; set; }
        public FilterByTime FilterByTime { get; set; }
    }
}
