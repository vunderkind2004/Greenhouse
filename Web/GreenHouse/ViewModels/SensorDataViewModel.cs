using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.ViewModels
{
    public class SensorDataViewModel
    {
        //public DateTime EventDateTime { get; set; }
        //public float Value { get; set; }
        //public string DeviceName { get; set; }
        //public int DeviceId {get;set;}
        //public string SensorType { get; set; }
        //public string SensorDimension { get; set; }

        public string[] Timestamps { get; set; }

        public DataSet[] DataSets { get; set; }
    }
}
