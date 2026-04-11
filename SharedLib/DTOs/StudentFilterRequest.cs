// ╔══════════════════════════════════════════════════╗
// ║  SharedLib/DTOs/StudentFilterRequest.cs           ║
// ╚══════════════════════════════════════════════════╝

namespace SharedLib.DTOs;

/// <summary>
/// طلب تصفية الطلاب مع Pagination
/// يُرسل من الـ Client إلى الـ API
/// </summary>
public class StudentFilterRequest
{
    /// <summary>الصفحة الحالية (تبدأ من 1)</summary>
    public int Page { get; set; } = 1;

    /// <summary>عدد العناصر في كل صفحة</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>نص البحث (اسم أو رقم هوية)</summary>
    public string? SearchText { get; set; }

    /// <summary>فلتر الجنس</summary>
    public string? Gender { get; set; }

    /// <summary>فلتر المستوى</summary>
    public string? Level { get; set; }
    public string? Center { get; set; }
    /// <summary>فلتر تاريخ الإضافة — من هذا التاريخ فصاعلاً</summary>
    public DateOnly? FromDate { get; set; } 
}