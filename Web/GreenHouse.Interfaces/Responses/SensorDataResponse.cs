using System;

namespace GreenHouse.Interfaces.Responses
{
    public class SensorDataResponse
    {
        public DateTime EventDateTime { get; set; }
        public float Value { get; set; }

        public string SensorName { get; set; }
        public string Dimension { get; set; }

        public string DeviceName { get; set; }

        public string DataSetName => NameHelper.GetDataChartLineName(DeviceName, SensorName, Dimension);
    }
}
