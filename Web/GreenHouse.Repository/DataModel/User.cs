using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.DataAnnotations;

namespace GreenHouse.Repository.DataModel
{
    public class User : BaseModel
    {
        public DateTime RegistrationDate { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public bool IsAdmin { get; set; }
    }
}
