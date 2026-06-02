using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    // Models/BackupRequest.cs
    public class BackupRequest
    {
        public string BackupName { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string BackupType { get; set; } = "Full";
        public string Format { get; set; } = "bak";
        public string SavePath { get; set; } = string.Empty;
        public bool Compress { get; set; } = true;
        public bool Encrypt { get; set; } = false;
        public bool VerifyAfter { get; set; } = true;
        public bool NotifyOnComplete { get; set; } = true;
        public List<string> SelectedTables { get; set; } = [];
    }
}
