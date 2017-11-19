using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenHouse.Helpers
{
    public class RgbHelper
    {
        private float MinTemperature { get; }
        private float MaxTemperature { get; }
        private float dTMax { get; }
        

        public RgbHelper(float minTemperature, float maxTemperature)
        {
            MinTemperature = minTemperature;
            MaxTemperature = maxTemperature;

            dTMax = maxTemperature - minTemperature;
        }

        public RGB ConvertFromTemperature(float temperature)
        {
            if (dTMax == 0)
                return new RGB { Blue = 100, Green = 100, Red = 100 };

            var dt = temperature - MinTemperature;
            if (dt == 0)
                return new RGB { Blue = byte.MaxValue };

            var b = Math.Cos((Math.PI / 2) * (dt / dTMax));
            var r = Math.Sin((Math.PI / 2) * (dt / dTMax));
            var g = Math.Sin((Math.PI / 2) * (0.5 + dt / dTMax));

            return new RGB
            {
                Red = (byte) (r * byte.MaxValue),
                Green = (byte)(g * byte.MaxValue),
                Blue = (byte)(b * byte.MaxValue)
            };
        }
    }

    public class RGB
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }
    }
}