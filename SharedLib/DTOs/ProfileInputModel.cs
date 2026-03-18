using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedLib.DTOs
{
    public class ProfileInputModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmpName { get; set; }
        public string? EmpId { get; set; }
       // public Employee? employee { get; set; }
        public string? Role { get; set; }

    }
}
