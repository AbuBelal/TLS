using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLSClientSharedLib.ViewModels
{
    public class EmployeeAttendanceVM
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public List<DayVM> Days { get; set; } = new List<DayVM>();

        // نحتفظ بالنسخة الأصلية لنحدث عليها عند الحفظ
        public AttendanceRecord OriginalRecord { get; set; }
    }

    public class DayVM
    {
        public int DayNumber { get; set; }

        private bool? _isAttendant;
        public bool? IsAttendant
        {
            get => _isAttendant;
            set
            {
                _isAttendant = value;
                // ذكاء برمجي: إذا كان حاضراً، نقوم بتفريغ نوع الإجازة تلقائياً
                if (value == true)
                {
                    DescId = null;
                }
            }
        }

        public long? DescId { get; set; }
    }
}
