// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Repositories/Implemntations/AdminDashboardRepository.cs ║
// ╚══════════════════════════════════════════════════════════════╝

using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;

namespace APIServerLib.Repositories.Implemntations;

public class AdminDashboardRepository : IAdminDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        // ── 1. جلب جميع المراكز ──────────────────────────────────────
        var centers = await _context.Centers.AsNoTracking().ToListAsync();

        // ── 2. جلب جميع StdCenters النشطة (ToDate == null)
        //       مع بيانات الطالب كاملة دفعةً واحدة ───────────────────
        var allStdCenters = await _context.StdCenters
            .AsNoTracking()
            .Where(sc => sc.ToDate == null)
            .Include(sc => sc.Student)
                .ThenInclude(s => s!.Gender)
            .Include(sc => sc.Student)
                .ThenInclude(s => s!.Level)
            .ToListAsync();

        // ── 3. جلب جميع EmpCenters النشطة (ToDate == null)
        //       مع بيانات الموظف ──────────────────────────────────────
        var allEmpCenters = await _context.EmpCenters
            .AsNoTracking()
            .Where(ec => ec.ToDate == null)
            .Include(ec => ec.Employee)
                .ThenInclude(e => e!.Gender)
            .ToListAsync();

        var now          = DateOnly.FromDateTime(DateTime.Now);
        var firstOfMonth = new DateOnly(now.Year, now.Month, 1);

        // حساب الفرق: السبت في C# يأخذ القيمة 6 في ترتيب DayOfWeek
        // نقوم بحساب الإزاحة لضمان العودة إلى أقرب يوم سبت مضى
        int diff = (now.DayOfWeek - DayOfWeek.Saturday + 7) % 7;

        DateOnly startOfWeek = now.AddDays(-diff);

        // ── 4. بناء ملخص كل مركز ────────────────────────────────────
        var levelColors = new[]
        {
            "bg-primary", "bg-success", "bg-info",
            "bg-warning",  "bg-danger","bg-primary","bg-secondary",
            "bg-danger", "bg-success", "bg-info",
        };
        //var levelColors = new[]
        //{
        //    "DeepSkyBlue",
        //    "DeepPink",
        //    "ForestGreen",
        //    "FireBrick",
        //    "DarkSeaGreen",
        //    "DarkSalmon",
        //    "DarkSlateBlue",
        //    "DarkOrange",
        //    "GreenYellow"
        //};

        var centerSummaries = new List<CenterSummaryDto>();

        foreach (var center in centers)
        {
            // طلاب هذا المركز
            var centerStudents = allStdCenters
                .Where(sc => sc.CenterId == center.Id)
                .Select(sc => sc.Student!)
                .ToList();

            var totalStd    = centerStudents.Count;
            var maleStd     = centerStudents.Count(s => s.Gender?.Name == "ذكر");
            var femaleStd   = centerStudents.Count(s => s.Gender?.Name == "أنثى");
            var specialStd  = centerStudents.Count(s => s.IsSpecialNeeds);
            var unrwaStd    = centerStudents.Count(s => s.IsUnrwa);

            // حساب الطلاب الجدد هذا الشهر عبر StdCenter.FromDate
            var newThisMonth = allStdCenters
                .Where(sc => sc.CenterId == center.Id && sc.FromDate >= firstOfMonth)
                .Count();

            var newThisWeek = allStdCenters
                .Where(sc => sc.CenterId == center.Id && sc.FromDate >= startOfWeek)
                .Count();

            // موظفو هذا المركز
            var centerEmps  = allEmpCenters
                .Where(ec => ec.CenterId == center.Id)
                .Select(ec => ec.Employee!)
                .ToList();

            var totalEmp    = centerEmps.Count;
            var maleEmp     = centerEmps.Count(e => e.Gender?.Name == "ذكر");
            var femaleEmp   = centerEmps.Count(e => e.Gender?.Name == "أنثى");

            // توزيع المستويات لهذا المركز
            var levelDist = centerStudents
                .Where(s => s.Level != null)
                .GroupBy(s => s.Level!.Name)
                .Select((g, i) => new DistributionItem
                {
                    Label      = g.Key ?? "-",
                    Count      = g.Count(),
                    Percent    = totalStd == 0 ? 0
                                 : (int)Math.Round((double)g.Count() / totalStd * 100),
                    ColorClass = i < levelColors.Length ? levelColors[i] : "bg-secondary"
                })
                .OrderBy(d => d.Order)
                .ToList();

            centerSummaries.Add(new CenterSummaryDto
            {
                CenterId            = center.Id,
                CenterName          = center.Name,
                CenterCode          = center.CenterCode,
                Address             = center.Address,
                DaysOfWeek          = center.DaysOfWeek,
                Rooms               = center.Rooms,
                Tarpaulins          = center.Tarpaulins,
                OtherSpaces         = center.OtherSpaces,

                TotalStudents       = totalStd,
                MaleStudents        = maleStd,
                FemaleStudents      = femaleStd,
                SpecialNeedsCount   = specialStd,
                UnrwaCount          = unrwaStd,
                NewStudentsThisMonth= newThisMonth,
                NewStudentsThisWeek = newThisWeek,

                TotalEmployees      = totalEmp,
                MaleEmployees       = maleEmp,
                FemaleEmployees     = femaleEmp,

                LevelDistribution   = levelDist
            });
        }

        // ── 5. الإجماليات الكلية ─────────────────────────────────────
        var allStudents  = allStdCenters.Select(sc => sc.Student!).ToList();
        var allEmployees = allEmpCenters.Select(ec => ec.Employee!).ToList();

        int grandTotal   = allStudents.Count;

        // توزيع المستويات الكلي
        var globalLevelDist = allStudents
            .Where(s => s.Level != null)
            .GroupBy(s => s.Level!.Name)
            .Select((g, i) => new DistributionItem
            {
                Label      = g.Key ?? "-",
                Count      = g.Count(),
                Percent    = grandTotal == 0 ? 0
                             : (int)Math.Round((double)g.Count() / grandTotal * 100),
                ColorClass = i < levelColors.Length ? levelColors[i] : "bg-secondary"
            })
            .OrderBy(d => d.Order)
            .ToList();

        return new AdminDashboardDto
        {
            TotalCenters        = centers.Count,

            TotalStudents       = grandTotal,
            TotalMaleStudents   = allStudents.Count(s => s.Gender?.Name == "ذكر"),
            TotalFemaleStudents = allStudents.Count(s => s.Gender?.Name == "أنثى"),
            TotalSpecialNeeds   = allStudents.Count(s => s.IsSpecialNeeds),
            TotalUnrwa          = allStudents.Count(s => s.IsUnrwa),

            TotalEmployees      = allEmployees.Count,
            TotalMaleEmployees  = allEmployees.Count(e => e.Gender?.Name == "ذكر"),
            TotalFemaleEmployees= allEmployees.Count(e => e.Gender?.Name == "أنثى"),

            LevelDistribution   = globalLevelDist,
            Centers             = centerSummaries
        };
    }
}
