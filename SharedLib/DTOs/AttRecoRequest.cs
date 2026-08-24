using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.DTOs
{
    public class AttRecoRequest
    {
        public long EmployeeId { get; set; }
        public long? CenterId { get; set; }
        public int Month  { get; set; } 
        public int Year { get; set; }
    }
}
