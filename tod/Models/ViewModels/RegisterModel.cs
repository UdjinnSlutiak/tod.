using System;
using System.ComponentModel.DataAnnotations;

namespace tod.Models.ViewModels
{
    public class RegisterModel
    {

        public int Id { get; set; }

        [Required (ErrorMessage = "Nickname field can not be empty")]
        [Display(Name = "Nickname")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "First name length have to be from 5 to 30 symbols")]
        public string Nickname { get; set; }

        [Required (ErrorMessage = "Email field can not be empty")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required (ErrorMessage = "Password field can not be empty")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
