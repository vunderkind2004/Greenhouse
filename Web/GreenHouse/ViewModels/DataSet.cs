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
    }

    
}
