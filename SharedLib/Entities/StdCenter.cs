using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;

namespace SharedLib.Entities
{
    public class StdCenter
    {
        public long StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        [JsonIgnore]
        public Student? Student { get; set; }

        public long CenterId { get; set; }
        [ForeignKey(nameof( CenterId))]
        //[JsonIgnore]
        public Center? Center { get; set; }

        ///
        public DateOnly FromDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.Date);
        public DateOnly? ToDate { get; set; }
        public String? Comments { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
