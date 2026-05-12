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
        [Required(ErrorMessage = "كلمة المرور مطلوبة عند إنشاء مستخدم جديد")]
        [StringLength(100, ErrorMessage = "كلمة المرور يجب أن تكون بين {2} و {1} حرفًا.", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
         ErrorMessage = "يجب أن تحتوي كلمة المرور على حرف كبير واحد على الأقل، حرف صغير، رقم، ورمز خاص.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "يجب تحديد صلاحية")]
        public string Role { get; set; } = "";

        public string? PhoneNumber { get; set; }

        // الموظف المرتبط
        public long? EmployeeId { get; set; }
    }
}