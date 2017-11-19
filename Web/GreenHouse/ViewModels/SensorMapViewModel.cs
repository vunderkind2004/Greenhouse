using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenHouse.ViewModels
{
    public class SensorMapViewModel
    {
        public IEnumerable<SensorMapInfo> SensorMapInfo { get; set; }
        public int TotalRowsCount { get; set; }
        public int TotalColumnsCount { get; set; }
    }
}