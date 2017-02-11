using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Interfaces.Responses
{
    public class SensorDataGrouped
    {
        public int SensorId { get; set; }
        public double AverageValue { get; set; }
        public DateTime? Date { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }            
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public int? Week { get; set; }

        public DateTime EventTime
        {
            get
            {
                var dateTime = Date ?? new DateTime(Year ?? DateTime.Now.Year, 1, 1);
                if (Date == null && Week != null)
                    dateTime = dateTime.AddDays((int)Week * 7);
                else if (Date == null && Month != null)
                    dateTime = dateTime.AddMonths((int)Month - 1);
                if (Hour != null)
                    dateTime = dateTime.AddHours((int)Hour);
                if (Minute != null)
                    dateTime = dateTime.AddMinutes((int)Minute);
                return dateTime;
            }
        }
    }
}
