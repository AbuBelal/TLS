using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class AttRecSummery
    {
        public long? CenterId { get; set; }

        public string? CenterName { get; set; }
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now).AddDays(1 - DateTime.Now.Day);

        public int? TotalEmployees { get; set; } = 0;
        public int? AttRecCompleted { get; set; } = 0;  
        public int? AttRecPartialCompleted =>TotalEmployees-AttRecCompleted;
    }
}
