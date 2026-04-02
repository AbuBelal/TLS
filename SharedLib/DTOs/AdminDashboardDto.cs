// ╔══════════════════════════════════════════════════════════════╗
// ║  SharedLib/DTOs/AdminDashboardDto.cs                         ║
// ║  DTOs الخاصة بـ Dashboard المدير العام                       ║
// ╚══════════════════════════════════════════════════════════════╝

namespace SharedLib.DTOs;

/// <summary>
/// إحصائيات عامة لجميع المراكز - يُستخدم في Dashboard المدير
/// </summary>
public class AdminDashboardDto
{
    // ── إحصائيات الطلاب الإجمالية ──────────────────────────────
    public int TotalStudents        { get; set; }
    public int TotalMaleStudents    { get; set; }
    public int TotalFemaleStudents  { get; set; }
    public int TotalSpecialNeeds    { get; set; }
    public int TotalUnrwa           { get; set; }

    // ── إحصائيات الموظفين الإجمالية ────────────────────────────
    public int TotalEmployees       { get; set; }
    public int TotalMaleEmployees   { get; set; }
    public int TotalFemaleEmployees { get; set; }

    // ── إجمالي المراكز ─────────────────────────────────────────
    public int TotalCenters { get; set; }

    // ── توزيع الطلاب حسب المستوى (عموم المراكز) ────────────────
    public List<DistributionItem> LevelDistribution { get; set; } = new();

    // ── بيانات كل مركز على حدة ─────────────────────────────────
    public List<CenterSummaryDto> Centers { get; set; } = new();
}

/// <summary>
/// ملخص مركز واحد ضمن Dashboard المدير
/// </summary>
public class CenterSummaryDto
{
    // معلومات المركز
    public long   CenterId   { get; set; }
    public string CenterName { get; set; } = string.Empty;
    public string? CenterCode { get; set; }
    public string? Address    { get; set; }
    public string? DaysOfWeek { get; set; }
    public int?   Rooms       { get; set; }
    public int?   Tarpaulins  { get; set; }
    public int?   OtherSpaces { get; set; }

    // إحصائيات الطلاب
    public int TotalStudents       { get; set; }
    public int MaleStudents        { get; set; }
    public int FemaleStudents      { get; set; }
    public int SpecialNeedsCount   { get; set; }
    public int UnrwaCount          { get; set; }
    public int NewStudentsThisMonth{ get; set; }

    // النسب المئوية (محسوبة)
    public double MalePercent    => TotalStudents == 0 ? 0 : Math.Round((double)MaleStudents   / TotalStudents * 100, 1);
    public double FemalePercent  => TotalStudents == 0 ? 0 : Math.Round((double)FemaleStudents / TotalStudents * 100, 1);
    public double SpecialPercent => TotalStudents == 0 ? 0 : Math.Round((double)SpecialNeedsCount / TotalStudents * 100, 1);
    public double UnrwaPercent   => TotalStudents == 0 ? 0 : Math.Round((double)UnrwaCount     / TotalStudents * 100, 1);

    // إحصائيات الموظفين
    public int TotalEmployees      { get; set; }
    public int MaleEmployees       { get; set; }
    public int FemaleEmployees     { get; set; }

    // توزيع الطلاب حسب المستوى
    public List<DistributionItem> LevelDistribution { get; set; } = new();
}
