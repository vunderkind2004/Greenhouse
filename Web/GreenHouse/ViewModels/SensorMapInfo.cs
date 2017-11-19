namespace GreenHouse.ViewModels
{
    public class SensorMapInfo : SensorViewModel
    {
        public float? CurrentState { get; set; }
        public string Dimension { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}