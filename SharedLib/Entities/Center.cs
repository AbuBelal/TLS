using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public  class Center: BaseEntity
    {
        public string? CenterCode { get; set; }
        public string? EnName { get; set; }
        public string? BuildingCode { get; set; }
        public int? SortOrder { get; set; } = 0;
        public int? Rooms { get; set; } = 0;
        public int? Tarpaulins { get; set; } = 0;
        public int? OtherSpaces { get; set; } = 0;
        public long? WhoursId { get; set; }
        [ForeignKey(nameof(WhoursId))]
        public LookupValue? Whours { get; set; }


        [Required]
        public string? DaysOfWeek { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        //Navigation properties


        public ICollection<EmpCenter> EmpCenters { get; set; } = new List<EmpCenter>();
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        //[NotMapped]
        //public Employee? Manager => EmpCenters.FirstOrDefault(e=>e.Employee.Job?.Name.Contains("مدير مركز")==true && e.IsActive)?.Employee;

    }
}
