using APIServerLib.Data;
using APIServerLib.Repositories.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SharedLib.DTOs;
using SharedLib.Entities;
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
            var AllCenterAvgAttendance = AllCentersDailyReports.GroupBy(x => x.ReportDate).Select(g => new { Date = g.Key, AvgAttendance = g.Average(x => x.AttPercentage) }).ToList();
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
                dailyAttendance.CenterAttendanceAvg = CenterDailyReport != null ? CenterDailyReport.AttPercentage : 0;
                dailyAttendance.AreaAttendanceAvg = AllCenterAvgAttendance.Where(x => x.Date == date).Select(x => x.AvgAttendance).FirstOrDefault();

                dailyAttendances.Add(dailyAttendance);
            }
            return dailyAttendances;
        }
        public async Task<List<AllCentersDailyAttendance>> GetAttendancesAllCentersAsync(DateOnly From, DateOnly To, string DaysOfWeek)
        {
            
            var AllCentersDailyReports = await _context.DailyReports.Where(x => x.ReportDate >= From && x.ReportDate <= To && x.Center.DaysOfWeek == DaysOfWeek).ToListAsync();
            //var CenterDailyReports =  AllCentersDailyReports.Where(x => x.CenterId == CenterId).ToList();
            var AllCenterAvgAttendance = AllCentersDailyReports.GroupBy(x => x.ReportDate).Select(g => new { Date = g.Key, AvgAttendance = g.Average(x => x.AttPercentage) }).ToList();
           
            List<AllCentersDailyAttendance> dailyAttendances = new List<AllCentersDailyAttendance>();
            int order = 1;
            for (DateOnly date = From; date <= To; date = date.AddDays(1))
            {
                var AreaAttendanceAvg = AllCenterAvgAttendance.Where(x => x.Date == date).Select(x => x.AvgAttendance).FirstOrDefault();
                if(AreaAttendanceAvg <= 0)
                    continue;
                AllCentersDailyAttendance dailyAttendance = new ();
               dailyAttendance.Order = order++;
                dailyAttendance.AreaAttendanceAvg = AreaAttendanceAvg;
                foreach (var Center in await _context.Centers.ToListAsync())
                {
                    var IsWorkingDay = await IsWorkDayAsync(Center.Id, date);

                    //if (!IsWorkingDay)
                    //    continue;
                    var CenterDailyReports = AllCentersDailyReports.Where(x => x.CenterId == Center.Id).ToList();
                    var CenterDailyReport = CenterDailyReports.FirstOrDefault(x =>x.CenterId==Center.Id && x.ReportDate == date);

                    dailyAttendance.Date = date;

                    CenterAttendance centerAttendance = new CenterAttendance
                    {
                        CenterId = Center.Id,
                        CenterName = Center.Name,
                        CenterAttendanceAvg = CenterDailyReport != null ? CenterDailyReport.AttPercentage : 0,
                        IsWorkingDay = IsWorkingDay,
                        DaysOfWeek = Center.DaysOfWeek ?? ""

                    };
                    dailyAttendance.CentersAttendance.Add(centerAttendance);

                }
                if(dailyAttendance.CentersAttendance.Count > 0)
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
  