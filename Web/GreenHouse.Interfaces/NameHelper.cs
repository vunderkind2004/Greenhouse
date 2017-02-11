namespace GreenHouse.Interfaces
{
    public static class NameHelper
    {
        public static string GetDataChartLineName(string deviceName, string sensorName, string dimension)
        {
            return $"{deviceName}. {sensorName}, {dimension}";
        }

    }
}