using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    // Models/BackupRecord.cs
    public class BackupRecord
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string BackupType { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public int SizeMB { get; set; }
        [NotMapped]
        public BackupStatus Status { get; set; } = BackupStatus.Success;
        public bool IsCompressed { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsVerified { get; set; }
        public string? Notes { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? DeletedAt { get; set; }
    }

    public enum BackupStatus { Success, Failed, VerifyFailed }
}
