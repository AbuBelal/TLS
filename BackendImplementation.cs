// This file contains the backend implementation that needs to be added to the API project
// Add this to your AdminDashboardController.cs or create a new controller

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
using YourDbContextNamespace; // Replace with your actual DbContext namespace

namespace YourApiNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminDashboardController : ControllerBase
    {
        private readonly YourDbContext _context; // Replace with your DbContext

        public AdminDashboardController(YourDbContext context)
        {
            _context = context;
        }

        [HttpGet("daily-report")]
        public async Task<ActionResult<DailyReportDto>> GetDailyReport([FromQuery] DateOnly? date = null)
        {
            var reportDate = date ?? DateOnly.FromDateTime(DateTime.Now);

            // Get all centers
            var centers = await _context.Centers.ToListAsync();

            var dailyReport = new DailyReportDto
            {
                ReportDate = reportDate,
                Centers = new List<DailyCenterReport>()
            };

            // Initialize level dictionaries
            var levelNames = await _context.LookupValues
                .Where(l => l.Type == "Level") // Adjust based on your lookup type
                .Select(l => l.Value)
                .ToListAsync();

            foreach (var level in levelNames)
            {
                dailyReport.LevelRegisteredMaleTotals[level] = 0;
                dailyReport.LevelRegisteredFemaleTotals[level] = 0;
                dailyReport.LevelAttendanceMaleTotals[level] = 0;
                dailyReport.LevelAttendanceFemaleTotals[level] = 0;
            }

            foreach (var center in centers)
            {
                var centerReport = new DailyCenterReport
                {
                    CenterId = center.Id,
                    CenterName = center.Name,
                    CenterCode = center.Code,
                    CenterManager = center.ManagerName,
                    WHoures = center.WorkingHours,
                    RegisteredLevelMales = new Dictionary<string, int>(),
                    RegisteredLevelFemales = new Dictionary<string, int>(),
                    AttendanceLevelMales = new Dictionary<string, int>(),
                    AttendanceLevelFemales = new Dictionary<string, int>()
                };

                // Initialize level counts for this center
                foreach (var level in levelNames)
                {
                    centerReport.RegisteredLevelMales[level] = 0;
                    centerReport.RegisteredLevelFemales[level] = 0;
                    centerReport.AttendanceLevelMales[level] = 0;
                    centerReport.AttendanceLevelFemales[level] = 0;
                }

                // Check if report is locked
                var lockRecord = await _context.DailyReportLocks
                    .FirstOrDefaultAsync(l => l.ReportDate == reportDate && l.CenterId == center.Id);
                centerReport.IsLocked = lockRecord?.IsLocked ?? false;

                // Get registered students for this center (current enrollments)
                var registeredStudents = await _context.StdCenters
                    .Where(sc => sc.CenterId == center.Id &&
                                (sc.ToDate == null || sc.ToDate >= reportDate) &&
                                sc.FromDate <= reportDate)
                    .Include(sc => sc.Student)
                    .ThenInclude(s => s.Gender)
                    .Include(sc => sc.Student.Level)
                    .Select(sc => sc.Student)
                    .ToListAsync();

                foreach (var student in registeredStudents)
                {
                    var levelName = student.Level?.Value ?? "غير محدد";
                    var genderName = student.Gender?.Value ?? "غير محدد";

                    if (genderName.Contains("ذكر") || genderName.Contains("male"))
                    {
                        centerReport.RegisteredLevelMales[levelName]++;
                        centerReport.TotalRegisteredMales++;
                    }
                    else if (genderName.Contains("أنثى") || genderName.Contains("female"))
                    {
                        centerReport.RegisteredLevelFemales[levelName]++;
                        centerReport.TotalRegisteredFemales++;
                    }
                }

                // Get attendance for today
                var attendanceRecords = await _context.Attendances
                    .Where(a => a.Date == reportDate && a.CenterId == center.Id)
                    .Include(a => a.Student)
                    .ThenInclude(s => s.Gender)
                    .Include(a => a.Student.Level)
                    .ToListAsync();

                foreach (var attendance in attendanceRecords.Where(a => a.IsPresent))
                {
                    var levelName = attendance.Student.Level?.Value ?? "غير محدد";
                    var genderName = attendance.Student.Gender?.Value ?? "غير محدد";

                    if (genderName.Contains("ذكر") || genderName.Contains("male"))
                    {
                        centerReport.AttendanceLevelMales[levelName]++;
                        centerReport.TotalAttendanceMales++;
                    }
                    else if (genderName.Contains("أنثى") || genderName.Contains("female"))
                    {
                        centerReport.AttendanceLevelFemales[levelName]++;
                        centerReport.TotalAttendanceFemales++;
                    }
                }

                // Update grand totals
                foreach (var level in levelNames)
                {
                    dailyReport.LevelRegisteredMaleTotals[level] += centerReport.RegisteredLevelMales[level];
                    dailyReport.LevelRegisteredFemaleTotals[level] += centerReport.RegisteredLevelFemales[level];
                    dailyReport.LevelAttendanceMaleTotals[level] += centerReport.AttendanceLevelMales[level];
                    dailyReport.LevelAttendanceFemaleTotals[level] += centerReport.AttendanceLevelFemales[level];
                }

                dailyReport.GrandTotalRegisteredMales += centerReport.TotalRegisteredMales;
                dailyReport.GrandTotalRegisteredFemales += centerReport.TotalRegisteredFemales;
                dailyReport.GrandTotalAttendanceMales += centerReport.TotalAttendanceMales;
                dailyReport.GrandTotalAttendanceFemales += centerReport.TotalAttendanceFemales;

                dailyReport.Centers.Add(centerReport);
            }

            return Ok(dailyReport);
        }

        [HttpPost("lock-daily-report")]
        public async Task<IActionResult> LockDailyReport([FromBody] DailyReportLockRequest request)
        {
            var lockRecord = await _context.DailyReportLocks
                .FirstOrDefaultAsync(l => l.ReportDate == request.ReportDate && l.CenterId == request.CenterId);

            if (lockRecord == null)
            {
                lockRecord = new DailyReportLock
                {
                    ReportDate = request.ReportDate,
                    CenterId = request.CenterId,
                    IsLocked = request.IsLocked,
                    LockedAt = DateTime.Now,
                    // Set LockedBy to current user ID
                };
                _context.DailyReportLocks.Add(lockRecord);
            }
            else
            {
                lockRecord.IsLocked = request.IsLocked;
                if (request.IsLocked)
                {
                    lockRecord.LockedAt = DateTime.Now;
                    // Update LockedBy
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}