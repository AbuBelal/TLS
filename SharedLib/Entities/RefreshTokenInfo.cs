using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Entities
{
    public class RefreshTokenInfo
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        public int UserId { get; set; }
    }
}
