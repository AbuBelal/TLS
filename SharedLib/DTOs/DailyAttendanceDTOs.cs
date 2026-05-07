using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class DailyAttendance
    {
        public int Order { get; set; } = 0;
        public DateOnly Date { get; set; }
        public int CenterAttendanceCount { get; set; } = 0;
        public double CenterAttendanceAvg { get; set; } = 0;
        public int AreaAttendanceCount { get; set; } = 0;
        public  double AreaAttendanceAvg { get; set; } = 0;
    }
    public class AllCentersDailyAttendance
    {
        public int Order { get; set; } = 0;
        public DateOnly Date { get; set; }
        public List<CenterAttendance> CentersAttendance { get; set; } = new List<CenterAttendance>();
        public int AreaAttendanceCount { get; set; } = 0;
        public  double AreaAttendanceAvg { get; set; } = 0;
    }

    public class AttendanceRequest
    {
        public long? CenterId { get; set; }=0;
        public DateOnly From { get; set; }= DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string DaysOfWeek { get; set; } = "السبت-الاثنين-الأربعاء";
    }

    public class CenterAttendance
    {
        public long CenterId { get; set; } = 0;
        public string? CenterName { get; set; }
        public int CenterAttendanceCount { get; set; } = 0;
        public double CenterAttendanceAvg { get; set; } = 0;
        public bool IsWorkingDay { get; set; } = false;
        public string DaysOfWeek { get; set; } = string.Empty;
    }
}
