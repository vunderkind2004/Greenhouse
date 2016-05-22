using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenHouse.ViewModels
{
    public class SensorViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Location { get; set; }
        public int DeviceId { get; set; }
        public string DeviceName { get;set;}

        public IEnumerable<DeviceViewModel> AveableDevices { get; set; }
    }
}