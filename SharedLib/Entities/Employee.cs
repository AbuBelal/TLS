using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public class Employee : BaseEntity
    {

        public string CivilId { get; set; } = string.Empty;
        public string EmpId { get; set; } = string.Empty;
        //public string ArName { get; set; } = string.Empty;
        public string? EnName { get; set; } 
        public string? Mobile { get; set; } 
        public string? Address { get; set; } 
        public DateOnly? BirthDate { get; set; }
        //gender
        //Job
        //Specialization
        public long? GenderId { get; set; }
        [ForeignKey(nameof(GenderId))]
        public LookupValue? Gender { get; set; }
        public long? JobId { get; set; }
        [ForeignKey(nameof(JobId))]
        public LookupValue? Job { get; set; }

        public long? OrgJobId { get; set; }
        [ForeignKey(nameof(OrgJobId))]
        public LookupValue? OrgJob { get; set; }

        public String? OrgSchool { get; set; }

        public long? SpecializationId { get; set; }
        [ForeignKey(nameof(SpecializationId))]
        public LookupValue? Specialization { get; set; }

        public ICollection<EmpCenter> EmpCenters { get; set; } = new List<EmpCenter>();
        // سنفترض هنا أنك ستستخدم نفس الـ Id الخاص بالمستخدم
        // أو يمكنك إضافة حقل منفصل
        public virtual ApplicationUser? User { get; set; }

    }
}
