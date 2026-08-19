using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class GenerateAttendanceRequest
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public long? HolidayCode { get; set; }
        public bool? Lock { get; set; } = true;
    }
}
