using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class UserProfileDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public long? EmployeeId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Role { get; set; }
        public long? CenterId { get; set; }
        public string? CenterName { get; set; }
    }
}
