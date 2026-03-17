using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class UserWithRoles:ApplicationUser
    {
        public List<string>? Roles { get; set; }
    }
}
