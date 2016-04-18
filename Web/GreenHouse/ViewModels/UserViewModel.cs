using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GreenHouse.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }

        public DateTime RegistrationDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MinLength(6)]
        [MaxLength(50)]
        public string Login { get; set; }

        [Display(Name = "Email address")]
        [Required(ErrorMessage = "The email address is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [MaxLength(50)]
        public string Email { get; set; }

        [MinLength(6)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string Phone { get; set; }

        public bool IsAdmin { get; set; }
    }
}