using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenHouse.ViewModels
{
    public class DeviceViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Token { get; set; }
    }
}