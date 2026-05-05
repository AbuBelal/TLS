using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIServerLib.Repositories.Implemntations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<DailyAttendance>> GetAttendancesAsync(long CenterId, DateOnly From, DateOnly To)
        {
            
            var AllCentersDailyReports = await _context.DailyReports.Where(x => x.ReportDate >= From && x.ReportDate <= To).ToListAsync();
            var CenterDailyReports =  AllCentersDailyReports.Where(x => x.CenterId == CenterId).ToList();
            var AllCenterAvgAttendance = AllCentersDailyReports.GroupBy(x => x.ReportDate).Select(g => new { Date = g.Key, AvgAttendance = g.Average(x => x.AttTotal) }).ToList();
            List<DailyAttendance> dailyAttendances = new List<DailyAttendance>();
            int order = 1;
            for (DateOnly date = From; date <= To; date = date.AddDays(1))
            {
                var IsWorkingDay = await IsWorkDayAsync(CenterId, date);
                
                if(!IsWorkingDay)
                    continue;

                var CenterDailyReport = CenterDailyReports.FirstOrDefault(x => x.ReportDate == date);
                DailyAttendance dailyAttendance = new DailyAttendance();
                dailyAttendance.Date = date;
                dailyAttendance.Order = order++;
                dailyAttendance.CenterAttendanceCount = CenterDailyReport != null ? CenterDailyReport.AttTotal : 0;
                dailyAttendance.AreaAttendanceCount = (int)AllCenterAvgAttendance.Where(x => x.Date == date).Select(x => x.AvgAttendance).FirstOrDefault();

                dailyAttendances.Add(dailyAttendance);
            }
            return dailyAttendances;
        }

        public async Task<bool> IsWorkDayAsync(long centerId, DateOnly date)
        {
            var Center = await _context.Centers.FindAsync(centerId);
            if (Center == null) return false;

            string dayInEnglish = date.ToString("dddd");
            string dayInArabic = SharedLib.Fixed.GlobalData.ArabicDays.GetValueOrDefault(dayInEnglish, dayInEnglish);
            if (Center?.DaysOfWeek?.Contains(dayInArabic) ?? false)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

    }
}
  