using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedLib.DTOs
{
    public class AdminUserInputModel
    {
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserName { get; set; } = "";

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "يجب أن تكون 6 أحرف على الأقل")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "يجب تحديد صلاحية")]
        public string Role { get; set; } = "";
        public long? EmployeeId { get; set; } = 0;
    }
}
