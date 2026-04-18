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
    public string CenterManager { get; set; } = string.Empty;
    public string? CenterCode { get; set; }
    public string? WHours { get; set; }
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
    public int NewStudentsThisWeek { get; set; }

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

// ── تقرير مفصل للمراكز ─────────────────────────────────────────────
public class DetailedCentersReportDto
{
    public List<DetailedCenterReport> Centers { get; set; } = new();
    public Dictionary<string, int> LevelMaleTotals { get; set; } = new();
    public Dictionary<string, int> LevelFemaleTotals { get; set; } = new();
    public int GrandTotalMales { get; set; }
    public int GrandTotalFemales { get; set; }
    public int GrandTotalUnrwa { get; set; }
    public int GrandTotalSpecialNeeds { get; set; }
    public int GrandTotalStudents { get; set; }
    public int GrandTotalRooms { get; set; }
    public int GrandTotalTarpaulins { get; set; }
    public int GrandTotalOtherSpaces { get; set; }
}

public class DetailedCenterReport
{
    public long CenterId { get; set; }
    public string CenterName { get; set; } = string.Empty;
    public string? CenterManager { get; set; } = string.Empty;
    public string? CenterCode { get; set; }
    public string? WHoures { get; set; }

    // أعداد حسب المستوى والجنس
    public Dictionary<string, int> LevelMales { get; set; } = new(); // key: level name, value: count
    public Dictionary<string, int> LevelFemales { get; set; } = new();

    public int UnrwaCount { get; set; }
    public int SpecialNeedsCount { get; set; }
    public int TotalMales { get; set; }
    public int TotalFemales { get; set; }
    public int TotalStudents => TotalMales + TotalFemales;
    public int TotalRooms { get; set; }
    public int TotalTarpaulins { get; set; }
    public int TotalOtherSpaces { get; set; }
}

// ── تقرير يومي للمراكز ─────────────────────────────────────────────
public class DailyReportDto
{
    public DateOnly ReportDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    public List<DailyCenterReport> Centers { get; set; } = new();
    public Dictionary<string, int> LevelRegisteredMaleTotals { get; set; } = new();
    public Dictionary<string, int> LevelRegisteredFemaleTotals { get; set; } = new();
    public Dictionary<string, int> LevelAttendanceMaleTotals { get; set; } = new();
    public Dictionary<string, int> LevelAttendanceFemaleTotals { get; set; } = new();

    public int GrandTotalRegisteredMales { get; set; }
    public int GrandTotalRegisteredFemales { get; set; }
    public int GrandTotalAttendanceMales { get; set; }
    public int GrandTotalAttendanceFemales { get; set; }
    public int GrandTotalRegisteredStudents { get; set; }
    public int GrandTotalAttendanceStudents { get; set; }
}

public class DailyCenterReport
{
    public long CenterId { get; set; }
    public string CenterName { get; set; } = string.Empty;
    public string? CenterManager { get; set; } = string.Empty;
    public string? CenterCode { get; set; }
    public string? WHoures { get; set; }
    public bool IsLocked { get; set; } = false;

    // Registered students by level and gender
    public Dictionary<string, int> RegisteredLevelMales { get; set; } = new();
    public Dictionary<string, int> RegisteredLevelFemales { get; set; } = new();

    // Today's attendance by level and gender
    public Dictionary<string, int> AttendanceLevelMales { get; set; } = new();
    public Dictionary<string, int> AttendanceLevelFemales { get; set; } = new();

    public int TotalRegisteredMales { get; set; }
    public int TotalRegisteredFemales { get; set; }
    public int TotalAttendanceMales { get; set; }
    public int TotalAttendanceFemales { get; set; }
    public int TotalRegisteredStudents => TotalRegisteredMales + TotalRegisteredFemales;
    public int TotalAttendanceStudents => TotalAttendanceMales + TotalAttendanceFemales;
}

public class DailyReportLockRequest
{
    public DateOnly ReportDate { get; set; }
    public long CenterId { get; set; }
    public bool IsLocked { get; set; }
}
