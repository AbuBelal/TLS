using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs.Students
{
    public class StudentPromotionRequest
    {
        public long FromLevelId { get; set; }
        public long ToLevelId { get; set; }
    }
}
