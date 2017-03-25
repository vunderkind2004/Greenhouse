using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenhouseApp.ViewModels
{
    public class SensorViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }

        public int SensorTypeId { get; set; }
        public SensorType SensorType { get; set; }

        public int DeviceId { get; set; }
        //public Device Device { get; set; }
    }

    
}
