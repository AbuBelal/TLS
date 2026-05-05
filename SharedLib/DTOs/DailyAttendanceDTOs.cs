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
        public int AreaAttendanceCount { get; set; } = 0;
    }

    public class AttendanceRequest
    {
        public long CenterId { get; set; }=0;
        public DateOnly From { get; set; }= DateOnly.FromDateTime(DateTime.Now.AddDays(30));
        public DateOnly To { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
