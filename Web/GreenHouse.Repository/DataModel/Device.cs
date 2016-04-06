using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenHouse.Repository.DataModel
{
    public class Device : BaseModel
    {
        public DateTime RegistrationDate { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Token { get; set; }
    }
}
