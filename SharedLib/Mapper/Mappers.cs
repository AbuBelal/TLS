using Riok.Mapperly.Abstractions;
using SharedLib.DTOs;
using SharedLib.Entities;

namespace SharedLib.Mappers;

[Mapper]
public partial class EmployeeMapper
{
    // DTO ➜ Entity (Insert)
    public partial Employee ToEntity(EmployeeUpsertDto dto);

    // DTO ➜ Entity موجود (Update) - يحدّث الخصائص مباشرة
    public partial void UpdateEntity(EmployeeUpsertDto dto, Employee employee);

    // Entity ➜ DTO (للقراءة)
    public partial EmployeeUpsertDto ToEmployeeUpsertDTO (Employee employee);
}