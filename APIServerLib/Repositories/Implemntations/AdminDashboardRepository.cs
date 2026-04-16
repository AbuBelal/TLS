// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Repositories/Implemntations/AdminDashboardRepository.cs ║
// ╚══════════════════════════════════════════════════════════════╝

using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Fixed;

namespace APIServerLib.Repositories.Implemntations;

public class AdminDashboardRepository : IAdminDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public AdminDashboardRepository(ApplicationDbContext context)
        => _context = context;

    public async Task<AdminDashboardDto> GetAdminDashboardAsync()
    {
        // ── 1. جلب جميع المراكز ──────────────────────────────────────
        var centers = await _context.Centers
            .Include(x=>x.Whours)
            .Include(s=>s.EmpCenters.Where(ec => ec.Employee.Job.Name== "مدير مركز")).ThenInclude(ec => ec.Employee)
            .AsNoTracking().ToListAsync();

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
            var Genders     = await  _context.LookupValues.Where(x => x.ValueType == LookupTypes.Gender).ToListAsync();
            Genders ??= new ();
            // توزيع المستويات لهذا المركز
            var levelDist = centerStudents
                .Where(s => s.Level != null)
                .GroupBy(s => s.Level!.Name)
                .Select((g, i) => new DistributionItem
                {
                    Label      = g.Key ?? "-",
                    Count      = g.Count(),
                    //GendersCount = new Dictionary<string, int> { { "", 0 }, { "", 0 } },
                    //new Dictionary<string, int> { { Genders.First().Name, g.Count(x=>x.Gender.Id== Genders.First().Id) },
                    //{ Genders.Last().Name, g.Count(x=>x.Gender.Id== Genders.Last().Id) }},
                    Percent = totalStd == 0 ? 0
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
                WHours              = center.Whours?.Name,
                CenterManager       = center.EmpCenters.FirstOrDefault()?.Employee?.Name ?? "غير محدد",

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
            .OrderBy(s => s.Level!.SortOrder) // ترتيب المستويات حسب SortOrder
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


    public async Task<DetailedCentersReportDto> GetDetailedCentersReportAsync()
    {
        // جلب جميع المراكز
        var centers = await _context.Centers.AsNoTracking().ToListAsync();

        // جلب جميع الطلاب النشطين مع البيانات المطلوبة
        var allStdCenters = await _context.StdCenters
            .AsNoTracking()
            .Where(sc => sc.ToDate == null)
            .Include(sc => sc.Student)
                .ThenInclude(s => s!.Gender)
            .Include(sc => sc.Student)
                .ThenInclude(s => s!.Level)
            .ToListAsync();

        // جلب المستويات مرتبة
        var levels = await _context.LookupValues
            .Where(lv => lv.ValueType == "Level")
            .OrderBy(lv => lv.SortOrder)
            .ToListAsync();

        var levelNames = levels.Select(l => l.Name).ToList();

        var report = new DetailedCentersReportDto();
        var levelMaleTotals = new Dictionary<string, int>();
        var levelFemaleTotals = new Dictionary<string, int>();

        foreach (var levelName in levelNames)
        {
            levelMaleTotals[levelName] = 0;
            levelFemaleTotals[levelName] = 0;
        }

        foreach (var center in centers)
        {
            var students = allStdCenters
                .Where(sc => sc.CenterId == center.Id)
                .Select(sc => sc.Student!)
                .ToList();

            var levelMales = new Dictionary<string, int>();
            var levelFemales = new Dictionary<string, int>();

            foreach (var levelName in levelNames)
            {
                levelMales[levelName] = students
                    .Count(s => s.Level?.Name == levelName && s.Gender?.Name == "ذكر");
                levelFemales[levelName] = students
                    .Count(s => s.Level?.Name == levelName && s.Gender?.Name == "أنثى");

                levelMaleTotals[levelName] += levelMales[levelName];
                levelFemaleTotals[levelName] += levelFemales[levelName];
            }

            var centerReport = new DetailedCenterReport
            {
                CenterId = center.Id,
                CenterName = center.Name,
                CenterCode = center.CenterCode,
                LevelMales = levelMales,
                LevelFemales = levelFemales,
                UnrwaCount = students.Count(s => s.IsUnrwa),
                SpecialNeedsCount = students.Count(s => s.IsSpecialNeeds),
                TotalMales = levelMales.Values.Sum(),
                TotalFemales = levelFemales.Values.Sum(),
                TotalRooms = center.Rooms??0,
                TotalTarpaulins = center.Tarpaulins??0,
                TotalOtherSpaces = center.OtherSpaces??0
            };

            report.Centers.Add(centerReport);
        }

        report.LevelMaleTotals = levelMaleTotals;
        report.LevelFemaleTotals = levelFemaleTotals;
        report.GrandTotalMales = levelMaleTotals.Values.Sum();
        report.GrandTotalFemales = levelFemaleTotals.Values.Sum();
        report.GrandTotalUnrwa = report.Centers.Sum(c => c.UnrwaCount);
        report.GrandTotalSpecialNeeds = report.Centers.Sum(c => c.SpecialNeedsCount);
        report.GrandTotalStudents = report.GrandTotalMales + report.GrandTotalFemales;

        report.GrandTotalRooms = report.Centers.Sum(c=>c.TotalRooms);
        report.GrandTotalTarpaulins = report.Centers.Sum(c => c.TotalTarpaulins);
        report.GrandTotalOtherSpaces = report.Centers.Sum(c => c.TotalOtherSpaces);

        return report;
    }
}
