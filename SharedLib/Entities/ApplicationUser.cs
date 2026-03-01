using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SharedLib.Entities
{
    public class ApplicationUser:IdentityUser
    {
        public long? EmployeeId { get; set; }

        // Navigation property for the Employee entity
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

    }
}
