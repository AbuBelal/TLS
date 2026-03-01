using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public  class Center: BaseEntity
    {
        public int? Rooms { get; set; } = 0;
        public int? Tarpaulins { get; set; } = 0;
        public int? OtherSpaces { get; set; } = 0;

        [Required]
        public string? DaysOfWeek { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;

        //Navigation properties
       
        
        public ICollection<EmpCenter> EmpCenters { get; set; } = new List<EmpCenter>();

    }
}
