using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class StudentsCountsRequestDto
    {
        public long? CenterId { get; set; }
        public DateOnly? From { get; set; } 
        public DateOnly? To { get; set; }
    }
    public class StudentsCountsDto
    {
        public DateOnly date { get; set; } 
        public int StdCount { get; set; } = 0;
    }
}
