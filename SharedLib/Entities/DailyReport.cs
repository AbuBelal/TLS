using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SharedLib.Entities
{
    /// <summary>
    /// Attendance record for a student on a specific date
    /// </summary>
    public class DailyReport
    {
       public long Id { get; set; }
        public DateOnly ReportDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public long CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        [JsonIgnore]
        public Center? Center { get; set; }
        public bool IsLocked { get; set; } = true;
        public DateTime? LockedAt { get; set; }
        public string? LockedBy { get; set; } // User ID who locked it


        //Registered students
        public int? RegMale01 { get; set; } = 0;
        public int? RegMale02 { get; set; } = 0;
        public int? RegMale03 { get; set; } = 0;
        public int? RegMale04 { get; set; } = 0;
        public int? RegMale05 { get; set; } = 0;
        public int? RegMale06 { get; set; } = 0;
        public int? RegMale07 { get; set; } = 0;
        public int? RegMale08 { get; set; } = 0;
        public int? RegMale09 { get; set; } = 0;
        public int? RegFemale01 { get; set; } = 0;
        public int? RegFemale02 { get; set; } = 0;
        public int? RegFemale03 { get; set; } = 0;
        public int? RegFemale04 { get; set; } = 0;
        public int? RegFemale05 { get; set; } = 0;
        public int? RegFemale06 { get; set; } = 0;
        public int? RegFemale07 { get; set; } = 0;
        public int? RegFemale08 { get; set; } = 0;
        public int? RegFemale09 { get; set; } = 0;

        // Attendant
       
        public int? AttMale01 { get; set; } = 0;
        public int? AttMale02 { get; set; } = 0;
        public int? AttMale03 { get; set; } = 0;
        public int? AttMale04 { get; set; } = 0;
        public int? AttMale05 { get; set; } = 0;
        public int? AttMale06 { get; set; } = 0;
        public int? AttMale07 { get; set; } = 0;
        public int? AttMale08 { get; set; } = 0;
        public int? AttMale09 { get; set; } = 0;
        public int? AttFemale01 { get; set; } = 0;
        public int? AttFemale02 { get; set; } = 0;
        public int? AttFemale03 { get; set; } = 0;
        public int? AttFemale04 { get; set; } = 0;
        public int? AttFemale05 { get; set; } = 0;
        public int? AttFemale06 { get; set; } = 0;
        public int? AttFemale07 { get; set; } = 0;
        public int? AttFemale08 { get; set; } = 0;
        public int? AttFemale09 { get; set; } = 0;

        //Calculated fields
        [NotMapped]
        public int RegStd01=> (RegMale01 ?? 0) + (RegFemale01 ?? 0);
        [NotMapped]
        public int RegStd02=> (RegMale02 ?? 0) + (RegFemale02 ?? 0);
        [NotMapped]
        public int RegStd03=> (RegMale03 ?? 0) + (RegFemale03 ?? 0);
        [NotMapped]
        public int RegStd04=> (RegMale04 ?? 0) + (RegFemale04 ?? 0);
        [NotMapped]
        public int RegStd05=> (RegMale05 ?? 0) + (RegFemale05 ?? 0);
        [NotMapped]
        public int RegStd06=> (RegMale06 ?? 0) + (RegFemale06 ?? 0);
        [NotMapped]
        public int RegStd07=> (RegMale07 ?? 0) + (RegFemale07 ?? 0);
        [NotMapped]
        public int RegStd08=> (RegMale08 ?? 0) + (RegFemale08 ?? 0);
        [NotMapped]
        public int RegStd09=> (RegMale09 ?? 0) + (RegFemale09 ?? 0);
        [NotMapped]

        public int AttStd01 => (AttMale01 ?? 0) + (AttFemale01 ?? 0);
        [NotMapped]
        public int AttStd02 => (AttMale02 ?? 0) + (AttFemale02 ?? 0);
        [NotMapped]
        public int AttStd03 => (AttMale03 ?? 0) + (AttFemale03 ?? 0);
        [NotMapped]
        public int AttStd04 => (AttMale04 ?? 0) + (AttFemale04 ?? 0);
        [NotMapped]
        public int AttStd05 => (AttMale05 ?? 0) + (AttFemale05 ?? 0);
        [NotMapped]
        public int AttStd06 => (AttMale06 ?? 0) + (AttFemale06 ?? 0);
        [NotMapped]
        public int AttStd07 => (AttMale07 ?? 0) + (AttFemale07 ?? 0);
        [NotMapped]
        public int AttStd08 => (AttMale08 ?? 0) + (AttFemale08 ?? 0);
        [NotMapped]
        public int AttStd09 => (AttMale09 ?? 0) + (AttFemale09 ?? 0);
        [NotMapped]
        public int RegTotal => RegStd01 + RegStd02 + RegStd03 + RegStd04 + RegStd05 + RegStd06 + RegStd07 + RegStd08 + RegStd09;
        [NotMapped]
        public int AttTotal => AttStd01 + AttStd02 + AttStd03 + AttStd04 + AttStd05 + AttStd06 + AttStd07 + AttStd08 + AttStd09;

    }
}
