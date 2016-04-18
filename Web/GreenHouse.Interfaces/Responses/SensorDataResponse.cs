﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.Responses
{
    public class SensorDataResponse
    {
        public DateTime EventDateTime { get; set; }
        public float Value { get; set; }

        public string TypeName { get; set; }
        public string Dimension { get; set; }

        public string DeviceName { get; set; }
    }
}