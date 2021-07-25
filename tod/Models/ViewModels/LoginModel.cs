using System;
using System.ComponentModel.DataAnnotations;

namespace tod.Models.ViewModels
{
    public class LoginModel
    {

        [Required(ErrorMessage = "Nickname field can not be empty")]
        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "Password field can not be empty")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
