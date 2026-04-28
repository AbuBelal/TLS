using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public  class Student:BaseEntity
    {
        [Length(9,9, ErrorMessage = "رقم الهوية يجب أن يكون 9 أرقام")]
        public string CivilId { get; set; } = string.Empty;
        public string? EnName { get; set; } = string.Empty;
        public string? Mobile { get; set; } 
        public DateOnly? BirthDate { get; set; }
        public short? SectionNo { get; set; } = 1;
        public bool IsUnrwa { get; set; }=false;
        public bool IsSpecialNeeds { get; set; }=false;
        public string? SpecialNeeds { get; set; }

        //gender
        //Level
        public long? GenderId { get; set; }
        [ForeignKey(nameof(GenderId))]
        public LookupValue? Gender { get; set; }

        public long? LevelId { get; set; }
        [ForeignKey(nameof(LevelId))]
        public LookupValue? Level { get; set; }

        // --- الربط مع جدول المراكز الوسطي StdCenter ---
        // هذه الخاصية تسمح للطالب بالارتباط بمركز واحد أو أكثر (سجل تاريخي)
       
        public ICollection<StdCenter> StdCenters { get; set; } = new List<StdCenter>();
    }
}
