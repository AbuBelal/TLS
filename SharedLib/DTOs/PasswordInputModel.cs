using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedLib.DTOs
{
    public class PasswordInputModel
    {
        [Required]
        public string UserId { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = "";

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "كلمات المرور غير متطابقة")]
        public string ConfirmPassword { get; set; } = "";
    }
}
