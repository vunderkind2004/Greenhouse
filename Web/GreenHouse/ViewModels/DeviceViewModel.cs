﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenHouse.ViewModels
{
    public class DeviceViewModel
    {
        [Required]
        public string Name { get; set; }
        public string Summary { get; set; }
    }
}