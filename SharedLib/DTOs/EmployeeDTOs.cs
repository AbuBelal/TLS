using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public sealed class EmployeeFilterRequest
    {
        public string SearchText { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Job { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public sealed class EmployeeListItemDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? EnName { get; set; }
        public string? CivilId { get; set; }
        public string? Mobile { get; set; }
        public string? GenderName { get; set; }
        public string? JobName { get; set; }
        public string? SpecializationName { get; set; }
    }

    public sealed class EmployeePaginatedResponse
    {
        public List<EmployeeListItemDto> Items { get; set; } = [];
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; } = 1;
        public List<string> GenderOptions { get; set; } = [];
        public List<string> JobOptions { get; set; } = [];
    }

    public class EmployeeUpsertDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CivilId { get; set; } = string.Empty;
        public string? EmpId { get; set; }
        public string? EnName { get; set; }
        public string? Mobile { get; set; }
        public string? Address { get; set; }
        public DateOnly? BirthDate { get; set; }
        public long? GenderId { get; set; }
        public long? JobId { get; set; }
        public long? OrgJobId { get; set; }
        public string? OrgSchool { get; set; }
        public long? SpecializationId { get; set; }
        public string? Comments { get; set; }
    }
}
