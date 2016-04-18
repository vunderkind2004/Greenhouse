using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace GreenHouse.Repository.DataModel
{
    public class Device : BaseModel
    {
        public DateTime RegistrationDate { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Token { get; set; }

        [References(typeof(User))]
        public int UserId { get; set; }
        [Reference]
        public User User { get; set; }
    }
}
