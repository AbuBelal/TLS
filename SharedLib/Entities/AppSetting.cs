using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Entities
{
    public class AppSetting
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public string SettingKey { get; set; } = string.Empty;
        public string? SettingValueStr { get; set; }
        public bool? SettingValueBool { get; set; }
        public string? Category { get; set; }
        public DateTime LastModified { get; set; } = DateTime.UtcNow;
    }
}
