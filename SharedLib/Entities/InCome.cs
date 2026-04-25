using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public  class InCome: BaseEntity
    {
        public DateOnly Date { get; set; }= DateOnly.FromDateTime(DateTime.Now);
        public decimal Qnty { get; set; } = 0;
        public long CenterId { get; set; }

        [ForeignKey("CenterId")]
        public Center Center { get; set; }
        public string? RecipientName { get; set; }
    }
}
