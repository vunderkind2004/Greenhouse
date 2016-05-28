using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GreenHouse.ViewModels
{
    public class DataSet
    {
        [JsonProperty("label")]
        public string Label { get; set; }
        [JsonProperty("data")]
        public float?[] Data { get; set; }
        [JsonProperty("fill")]
        public bool Fill { get; set; }

        //borderColor: "rgba(75,192,192,1)",
        [JsonProperty("borderColor")]
        public string LineColor { get; set; }

        [JsonProperty("pointBorderColor")]
        public string PointBorderColor { get; set; }
        [JsonProperty("pointRadius")]
        public int PointRadius { get { return 1; } }
    }

    
}
