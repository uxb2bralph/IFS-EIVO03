using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

using Business.Helper.Validation;

namespace eIVOGo.Models.ViewModel
{
    public class CbsLoginViewModel
    {
        [Required(ErrorMessage = "Please enter {0}")]
        [Display(Name = "PID")]
        //[EmailAddress]
        public string PID { get; set; }

        [Required(ErrorMessage = "Please enter {0}")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public String ReturnUrl { get; set; }
    }
    public class LoginViewModel : CbsLoginViewModel
    {

        [Display(Name = "ValidCode")]
        [CaptchaValidation("EncryptedCode", ErrorMessage = "驗證碼錯誤!!")]
        public string ValidCode { get; set; }

        [Display(Name = "EncryptedCode")]
        public string EncryptedCode { get; set; }

    }
}