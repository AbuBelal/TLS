using System;

namespace SharedLib.DTOs
{
    public class AuditLogDto
    {
        public long Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public string? EntityId { get; set; }
        public string? Details { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AuditLogFilterRequest
    {
        public string? SearchText { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public string? Details { get; set; }
        public string? UserId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool SortDescending { get; set; } = true;  // ← جديد: ترتيب تنازلي افتراضياً
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
