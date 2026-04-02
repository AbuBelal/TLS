using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class CenterDashboardDto
    {
        // معلومات المركز
        public long CenterId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public string? CenterCode { get; set; }
        public string? Address { get; set; }
        public string? DaysOfWeek { get; set; }
        public int? Rooms { get; set; }
        public int? Tarpaulins { get; set; }
        public int? OtherSpaces { get; set; }

        // إحصائيات الطلاب
        public int TotalStudents { get; set; }
        public int NewStudentsThisMonth { get; set; }
        public int SpecialNeedsCount { get; set; }
        public int UnrwaCount { get; set; }
        public double SpecialNeedsPercent => TotalStudents == 0 ? 0
            : Math.Round((double)SpecialNeedsCount / TotalStudents * 100, 1);
        public double UnrwaPercent => TotalStudents == 0 ? 0
            : Math.Round((double)UnrwaCount / TotalStudents * 100, 1);

        // إحصائيات الموظفين
        public int TotalEmployees { get; set; }

        // التوزيعات
        public List<DistributionItem> GenderDistribution { get; set; } = new();
        public List<DistributionItem> LevelDistribution { get; set; } = new();

        // القوائم
        public List<EmployeeSummary> Employees { get; set; } = new();
        public List<StudentSummary> RecentStudents { get; set; } = new();
    }

    public class DistributionItem
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public int Order { get; set; } = 0;
        public int Percent { get; set; }
        public string ColorClass { get; set; } = string.Empty;
    }

    public class EmployeeSummary
    {
        public string Name { get; set; } = string.Empty;
        public string? EmpId { get; set; }
        public string? Job { get; set; }
        public string? Specialization { get; set; }
        public string? Gender { get; set; }
    }

    public class StudentSummary
    {
        public string Name { get; set; } = string.Empty;
        public string? Level { get; set; }
        public string? Gender { get; set; }
        public bool IsUnrwa { get; set; }
        public bool IsSpecialNeeds { get; set; }
        public DateOnly EnrollDate { get; set; }
    }
}
