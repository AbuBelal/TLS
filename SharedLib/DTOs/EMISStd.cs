using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.DTOs
{
    public class StudentResponse
    {
        public List<StudentInfo> aaData { get; set; } = new();
    }

    public class StudentInfo
    {
        public string StudentId { get; set; }
        public string ArabicFullNameWithoutQuoted { get; set; }
        public string School { get; set; }
        public string Grade { get; set; }
        public string Section { get; set; }
    }
}
