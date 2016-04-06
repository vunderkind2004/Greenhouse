using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace GreenHouse.Repository.DataModel
{
    public class BaseModel
    {
        [AutoIncrement]
        public int Id { get; set; }
    }
}
