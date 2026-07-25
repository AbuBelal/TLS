using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public class WReport
    {
        [Key]
        public long Id { get; set; }

        public DateTime? WReportBegin { get; set; }

        public DateTime? WReportEnd { get; set; }

        public int? WReportNo { get; set; }

        [StringLength(150)]
        public string? Comments { get; set; }

        public virtual ICollection<WReportDetail> WReportDetails { get; set; } = new List<WReportDetail>();
    }
}
