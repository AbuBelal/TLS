using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs.Centers
{
    public class CentersInfo
    {
        public long CenterId { get; set; }
        public string CenterArName { get; set; }= string.Empty;
        public List<CenterLevels> Levels { get; set; } = new List<CenterLevels>();
    }

    public class CenterLevels
    {
        public long? LevelId { get; set; }
        public string LevelName { get; set; }= string.Empty;
        public List<short?> Sections { get; set; }=new List<short?>();
    }
}
