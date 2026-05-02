using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public  class EmpCenter
    {
        public long EmployeeId { get; set; }
        [ForeignKey(nameof (EmployeeId))]
        public Employee Employee { get; set; }

        public long CenterId { get; set; }
        [ForeignKey(nameof(CenterId))]
        public Center Center { get; set; }

        ///
        public DateOnly FromDate {  get; set; }=DateOnly.FromDateTime( DateTime.Now.Date);
        public DateOnly? ToDate {  get; set; }
        public String? Comments { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
