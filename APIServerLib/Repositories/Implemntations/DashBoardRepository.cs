using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;

namespace APIServerLib.Services;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;
    public DashboardRepository(ApplicationDbContext context) => _context = context;

    public async Task<CenterDashboardDto?> GetCenterDashboardAsync(string userId)
    {
        // 1. جلب الموظف المرتبط بالمستخدم
        var employee = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => u.Employee)
            .FirstOrDefaultAsync();

        if (employee is null) return null;

        // 2. جلب المركز النشط للموظف (آخر EmpCenter بدون ToDate)
        var empCenter = await _context.EmpCenters
            .Include(ec => ec.Center)
            .Where(ec => ec.EmployeeId == employee.Id && ec.ToDate == null)
            .OrderByDescending(ec => ec.FromDate)
            .FirstOrDefaultAsync();

        if (empCenter is null) return null;

        var centerId = empCenter.CenterId;
        var center = empCenter.Center;

        // 3. جلب طلاب المركز
        var students = await _context.StdCenters
            .Include(sc => sc.Student)
                .ThenInclude(s => s.Gender)
            .Include(sc => sc.Student)
                .ThenInclude(s => s.Level)
            .Where(sc => sc.CenterId == centerId && sc.ToDate == null)
            .Select(sc => sc.Student)
            .ToListAsync();

        // 4. جلب موظفي المركز
        var centerEmployees = await _context.EmpCenters
            .Include(ec => ec.Employee)
                .ThenInclude(e => e.Job)
            .Include(ec => ec.Employee)
                .ThenInclude(e => e.Specialization)
            .Include(ec => ec.Employee)
                .ThenInclude(e => e.Gender)
            .Where(ec => ec.CenterId == centerId && ec.ToDate == null)
            .Select(ec => ec.Employee)
            .ToListAsync();

        var total = students.Count;
        var now = DateOnly.FromDateTime(DateTime.Now);
        var firstOfMonth = new DateOnly(now.Year, now.Month, 1);

        // 5. توزيع الجنس
        var maleCount = students.Count(s => s.Gender?.Name == "ذكر");
        var femaleCount = students.Count(s => s.Gender?.Name == "أنثى");

        var genderDist = new List<DistributionItem>
        {
            new() {
                Label = "ذكور", Count = maleCount,
                Percent = total == 0 ? 0 : (int)Math.Round((double)maleCount / total * 100),
                ColorClass = "bg-primary"
            },
            new() {
                Label = "إناث", Count = femaleCount,
                Percent = total == 0 ? 0 : (int)Math.Round((double)femaleCount / total * 100),
                ColorClass = "bg-danger"
            },
        };

        // 6. توزيع المستويات
        var levelColors = new[] {
            "bg-indigo-500","bg-primary","bg-info",
            "bg-teal","bg-success","bg-warning","bg-danger"
        };

        var levelDist = students
            .Where(s => s.Level != null)
            .GroupBy(s => s.Level!.Name)
            .Select((g, i) => new DistributionItem
            {
                Label = g.Key ?? "-",
                Count = g.Count(),
                Percent = total == 0 ? 0 : (int)Math.Round((double)g.Count() / total * 100),
                ColorClass = i < levelColors.Length ? levelColors[i] : "bg-secondary"
            })
            .ToList();

        return new CenterDashboardDto
        {
            CenterId = center.Id,
            CenterName = center.Name,
            CenterCode = center.CenterCode,
            Address = center.Address,
            DaysOfWeek = center.DaysOfWeek,
            Rooms = center.Rooms,
            Tarpaulins = center.Tarpaulins,
            OtherSpaces = center.OtherSpaces,

            TotalStudents = total,
            NewStudentsThisMonth = students
                .Count(s => /* جلب تاريخ التسجيل من StdCenter */ true), // عدّل حسب مشروعك
            SpecialNeedsCount = students.Count(s => s.IsSpecialNeeds == true),
            UnrwaCount = students.Count(s => s.IsUnrwa == true),

            TotalEmployees = centerEmployees.Count,

            GenderDistribution = genderDist,
            LevelDistribution = levelDist,

            Employees = centerEmployees.Select(e => new EmployeeSummary
            {
                Name = e.Name,
                EmpId = e.EmpId,
                Job = e.Job?.Name,
                Specialization = e.Specialization?.Name,
                Gender = e.Gender?.Name,
            }).Take(10).ToList(),

            RecentStudents = students
                .OrderByDescending(s => s.Id)
                .Take(5)
                .Select(s => new StudentSummary
                {
                    Name = s.Name,
                    Level = s.Level?.Name,
                    Gender = s.Gender?.Name,
                    IsUnrwa = s.IsUnrwa == true,
                    IsSpecialNeeds = s.IsSpecialNeeds == true,
                    EnrollDate = now,
                }).ToList(),
        };
    }
}