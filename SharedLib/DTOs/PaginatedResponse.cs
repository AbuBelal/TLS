// ╔══════════════════════════════════════════════════╗
// ║  SharedLib/DTOs/PaginatedResponse.cs              ║
// ╚══════════════════════════════════════════════════╝

namespace SharedLib.DTOs;

/// <summary>
/// استجابة مقسّمة إلى صفحات - Generic
/// يُعاد من الـ API إلى الـ Client
/// </summary>
public class PaginatedResponse<T>
{
    /// <summary>عناصر الصفحة الحالية</summary>
    public List<T> Items { get; set; } = [];

    /// <summary>إجمالي عدد العناصر (بعد الفلترة)</summary>
    public int TotalCount { get; set; }

    /// <summary>الصفحة الحالية</summary>
    public int CurrentPage { get; set; }

    /// <summary>عدد العناصر في كل صفحة</summary>
    public int PageSize { get; set; }

    /// <summary>إجمالي عدد الصفحات</summary>
    public int TotalPages =>
        (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>هل يوجد صفحة سابقة؟</summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>هل يوجد صفحة تالية؟</summary>
    public bool HasNext => CurrentPage < TotalPages;
}
