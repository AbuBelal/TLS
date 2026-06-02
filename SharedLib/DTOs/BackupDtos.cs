using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedLib.DTOs
{
    public class BackupFormModel
    {
        [Required(ErrorMessage = "اسم النسخة مطلوب")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "الاسم بين 3 و 200 حرف")]
        [RegularExpression(@"^[a-zA-Z0-9_\-]+$",
            ErrorMessage = "الاسم يقبل فقط حروف إنجليزية وأرقام و _ و -")]
        public string BackupName { get; set; } = $"backup_{DateTime.Now:yyyy_MM_dd_HHmm}";

        [Required(ErrorMessage = "يرجى اختيار قاعدة البيانات")]
        public string DatabaseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى اختيار نوع النسخة")]
        public string BackupType { get; set; } = "Full";

        [Required(ErrorMessage = "يرجى اختيار صيغة الملف")]
        public string Format { get; set; } = "bak";

        [Required(ErrorMessage = "مسار الحفظ مطلوب")]
        [StringLength(500, ErrorMessage = "المسار طويل جداً")]
        public string SavePath { get; set; } = @"D:\Backups\TempEdu\";

        public bool Compress { get; set; } = true;
        public bool Encrypt { get; set; } = false;
        public bool VerifyAfter { get; set; } = true;
        public bool NotifyOnComplete { get; set; } = true;

        // تحويل النموذج إلى Request جاهز للإرسال للـ API
        public BackupRequest ToRequest(List<string> selectedTables) => new()
        {
            BackupName = BackupName,
            DatabaseName = DatabaseName,
            BackupType = BackupType,
            Format = Format,
            SavePath = SavePath,
            Compress = Compress,
            Encrypt = Encrypt,
            VerifyAfter = VerifyAfter,
            NotifyOnComplete = NotifyOnComplete,
            SelectedTables = selectedTables,
        };

        // إعادة تعيين القيم الافتراضية
        public void Reset()
        {
            BackupName = $"backup_{DateTime.Now:yyyy_MM_dd_HHmm}";
            BackupType = "Full";
            Format = "bak";
            SavePath = @"D:\Backups\TempEdu\";
            Compress = true;
            Encrypt = false;
            VerifyAfter = true;
            NotifyOnComplete = true;
        }
    }
    public record DatabaseInfo(string Name, string DisplayName);

}
