using System.ComponentModel.DataAnnotations;

namespace SharedLib.DTOs
{
    public class UserUpsertDto
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد غير صحيحة")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        public string UserName { get; set; } = "";

        // كلمة المرور مطلوبة فقط في حالة الإضافة
        public string? Password { get; set; }

        [Required(ErrorMessage = "يجب تحديد صلاحية")]
        public string Role { get; set; } = "";

        public string? PhoneNumber { get; set; }

        // الموظف المرتبط
        public long? EmployeeId { get; set; }
    }
}