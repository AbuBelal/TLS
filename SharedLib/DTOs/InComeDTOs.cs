using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SharedLib.DTOs
{
    // DTOs/InComeDto.cs
    public record InComeDto(
        long Id,
        string Name,
        string? Comments,
        DateOnly Date,
        decimal Qnty,
        long CenterId,
        string? CenterName,
        string? RecipientName
    );

    public record CreateInComeDto(
        [Required(ErrorMessage = "الاسم مطلوب")]
    [StringLength(250)]
    string Name,

        string? Comments,

        [Required]
    DateOnly Date,

        [Range(0, double.MaxValue, ErrorMessage = "المبلغ يجب أن يكون موجباً")]
    decimal Qnty,

        [Required(ErrorMessage = "يجب اختيار المركز")]
    long CenterId,

        string? RecipientName
    );

    public record UpdateInComeDto(
        long Id,
        [Required]
    [StringLength(250)]
    string Name,

        string? Comments,
        DateOnly Date,

        [Range(0, double.MaxValue)]
    decimal Qnty,

        [Required]
    long CenterId,

        string? RecipientName
    );

    public class InComeFormDto
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "اسم الإيراد مطلوب")]
        [StringLength(250, ErrorMessage = "الاسم طويل جداً")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "يجب اختيار المركز")]
        [Range(1, long.MaxValue, ErrorMessage = "يجب اختيار مركز صحيح")]
        public long CenterId { get; set; }

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0.01, double.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من صفر")]
        public decimal Qnty { get; set; }

        [Required(ErrorMessage = "التاريخ مطلوب")]
        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        [StringLength(250)]
        public string? RecipientName { get; set; }

        public string? Comments { get; set; }
    }
}
