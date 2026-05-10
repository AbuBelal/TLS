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

    public class StudentEmisRequest
    {
        public string IdentityNo { get; set; }
        public string BirthYear { get; set; }
    }

    public class StudentEmisDto
    {
        public string StudentId { get; set; }
        public string IdentityNo { get; set; }
        public string FullName { get; set; }
        public string ResultStatus { get; set; }
        public string SchoolName { get; set; }
        public string Grade { get; set; }
        public string Section { get; set; }
        public string Mobile { get; set; }
        public string WhatsAppGroup { get; set; }
    }
}
