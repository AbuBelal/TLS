// ╔══════════════════════════════════════════════════╗
// ║  SharedLib/DTOs/StudentDto.cs                     ║
// ╚══════════════════════════════════════════════════╝

namespace SharedLib.DTOs;

/// <summary>
/// DTO للطالب - يُستخدم في الاستجابات
/// </summary>
public class StudentDto
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string? EnName { get; set; }
    public string? CivilId { get; set; }
    public string? Mobile { get; set; }
    public string? GenderName { get; set; }
    public string? LevelName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? CenterName { get; set; }
    public bool IsUnrwa { get; set; }
    public bool IsSpecialNeeds { get; set; }
}