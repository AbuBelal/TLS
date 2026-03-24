using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SharedLib.Entities
{
    public class LookupValue: BaseEntity
    {
        public string ValueType { get; set; } = string.Empty; //Type(Gender, Job, Specialization)
        public int? SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
