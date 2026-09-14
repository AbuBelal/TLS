using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs.Students
{
    public class MovStdToCenter
    {
        public long FromCenterId { get; set; }
        public long ToCenterId { get; set; }
        public long LevelId { get; set; }
        public short? Section { get; set; }
        public long? GenderId { get; set; } = 0;
    }
}
