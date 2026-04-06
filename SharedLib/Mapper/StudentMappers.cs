using Riok.Mapperly.Abstractions;
using SharedLib.DTOs;
using SharedLib.Entities;
using System.Runtime.ConstrainedExecution;

namespace SharedLib.Mappers;

[Mapper]
public partial class StudentMapper
{
    // DTO ➜ Entity (Insert)
    public partial Student ToEntity(StudentDto dto);
    // DTO ➜ Entity موجود (Update) - يحدّث الخصائص مباشرة
    public partial void UpdateEntity(StudentDto dto, Student student);
    // Entity ➜ DTO (للقراءة)
    public partial StudentDto ToStudentDTO(Student student);
}
 