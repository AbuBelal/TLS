using Riok.Mapperly.Abstractions;
using SharedLib.DTOs;
using SharedLib.Entities;

namespace SharedLib.Mappers;
[Mapper]
public partial class AttRecMappers
{
    // DTO ➜ Entity (Insert)
    public partial AttendanceRecord ToEntity(AttendanceRecordDto dto);

    // DTO ➜ Entity موجود (Update) - يحدّث الخصائص مباشرة
    public partial void UpdateEntity(AttendanceRecordDto dto, AttendanceRecord AttRec);

    // Entity ➜ DTO (للقراءة)
    public partial AttendanceRecordDto ToDTO (AttendanceRecord AttRec);
}
