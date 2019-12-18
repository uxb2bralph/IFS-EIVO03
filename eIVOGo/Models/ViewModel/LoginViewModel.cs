using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Business.Helper.Validation;

namespace eIVOGo.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "PID")]
        //[EmailAddress]
        public string PID { get; set; }        

        [Display(Name = "EncryptedCode")]
        public string EncryptedCode { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public String ReturnUrl { get; set; }
    }
}