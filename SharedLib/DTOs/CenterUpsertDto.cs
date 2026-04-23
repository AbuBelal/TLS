// ╔══════════════════════════════════════════════════════════════╗
// ║  SharedLib/DTOs/CenterUpsertDto.cs                           ║
// ╚══════════════════════════════════════════════════════════════╝

using System.ComponentModel.DataAnnotations;

namespace SharedLib.DTOs;

/// <summary>
/// DTO لتعديل بيانات المركز — يُرسل من الـ Client إلى API
/// </summary>
public class CenterUpsertDto
{
    public long Id { get; set; }

    [Required(ErrorMessage = "اسم المركز مطلوب")]
    [MaxLength(200, ErrorMessage = "الاسم لا يتجاوز 200 حرف")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "الكود لا يتجاوز 50 حرفاً")]
    public string? CenterCode { get; set; }

    [MaxLength(500, ErrorMessage = "العنوان لا يتجاوز 500 حرف")]
    public string? Address { get; set; }
    public long? WHours { get; set; }

    [Required(ErrorMessage = "أيام الدوام مطلوبة")]
    [MaxLength(100, ErrorMessage = "أيام الدوام لا تتجاوز 100 حرف")]
    public string? DaysOfWeek { get; set; }

    [Range(0, 999, ErrorMessage = "عدد الغرف يجب أن يكون بين 0 و 999")]
    public int? Rooms { get; set; } = 0;

    [Range(0, 999, ErrorMessage = "عدد الخيام يجب أن يكون بين 0 و 999")]
    public int? Tarpaulins { get; set; } = 0;

    [Range(0, 999, ErrorMessage = "المساحات الأخرى يجب أن تكون بين 0 و 999")]
    public int? OtherSpaces { get; set; } = 0;

    [MaxLength(1000)]
    public string? Comments { get; set; }
    public string? EnName { get; set; }
    public string? BuildingCode { get; set; }
    public int? SortOrder { get; set; } = 0;
}
