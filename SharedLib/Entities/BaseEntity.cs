using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedLib.Entities
{
    public class BaseEntity
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }= string.Empty;
        public string? Comments { get; set; }
    }
}
