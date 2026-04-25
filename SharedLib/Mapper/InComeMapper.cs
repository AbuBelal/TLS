using Riok.Mapperly.Abstractions;
using SharedLib.DTOs;
using SharedLib.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Mapper; 
    [Mapper]
    public partial class InComeMapper
    {
    // DTO ➜ Entity (Insert)
    public partial InCome ToEntity(InComeDto dto);

    // DTO ➜ Entity موجود (Update) - يحدّث الخصائص مباشرة
    public partial void UpdateEntity(InComeDto dto, InCome income);

    // Entity ➜ DTO (للقراءة)
    public partial InComeDto ToInComeDto(InCome income);

}

