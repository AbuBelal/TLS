// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Services/StudentExportService.cs               ║
// ║  يحوّل قائمة الطلاب إلى ملف Excel باستخدام ClosedXML        ║
// ╚══════════════════════════════════════════════════════════════╝

using ClosedXML.Excel;
using SharedLib.Entities;

namespace APIServerLib.Services;

public static class StudentExportService
{
    /// <summary>
    /// يُنشئ ملف Excel من قائمة طلاب ويُعيده كـ byte[]
    /// </summary>
    public static byte[] GenerateExcel(List<Student> students, string sheetTitle, string centerName)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("الطلاب");

        // ── إعداد اتجاه RTL للورقة ─────────────────────────────
        ws.RightToLeft = true;

        // ══════════════════════════════════════════════════════
        //  الصف الأول: عنوان التقرير (مدمج)
        // ══════════════════════════════════════════════════════
        int totalCols = 10;
        ws.Range(1, 1, 1, totalCols).Merge();
        var titleCell = ws.Cell(1, 1);
        titleCell.Value = $"قائمة الطلاب — {centerName}";
        titleCell.Style.Font.Bold      = true;
        titleCell.Style.Font.FontSize  = 16;
        titleCell.Style.Font.FontColor = XLColor.White;
        titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#009EDB");
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleCell.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
        ws.Row(1).Height = 30;

        // ══════════════════════════════════════════════════════
        //  الصف الثاني: معلومات التقرير (مدمج)
        // ══════════════════════════════════════════════════════
        ws.Range(2, 1, 2, totalCols).Merge();
        var infoCell = ws.Cell(2, 1);
        infoCell.Value = $"{sheetTitle}     |     تاريخ التصدير: {DateTime.Now:dd/MM/yyyy HH:mm}     |     العدد: {students.Count} طالب";
        infoCell.Style.Font.FontSize  = 10;
        infoCell.Style.Font.Italic    = true;
        infoCell.Style.Font.FontColor = XLColor.FromHtml("#555555");
        infoCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        infoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Row(2).Height = 18;

        // ══════════════════════════════════════════════════════
        //  الصف الثالث: رؤوس الأعمدة
        // ══════════════════════════════════════════════════════
        int headerRow = 3;
        var headers = new[]
        {
            ("#",           6),
            ("المركز",           25),
            ("الاسم بالعربي",  22),
            ("الاسم بالإنجليزي", 20),
            ("رقم الهوية",   14),
            ("رقم الجوال",   13),
            ("الجنس",         8),
            ("المستوى",       10),
            ("الأنروا",       8),
            ("احتياجات خاصة", 12),
        };

        for (int c = 0; c < headers.Length; c++)
        {
            var cell = ws.Cell(headerRow, c + 1);
            cell.Value = headers[c].Item1;
            cell.Style.Font.Bold      = true;
            cell.Style.Font.FontSize  = 11;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#005577");
            ws.Column(c + 1).Width = headers[c].Item2;
        }
        ws.Row(headerRow).Height = 22;

        // ══════════════════════════════════════════════════════
        //  بيانات الطلاب
        // ══════════════════════════════════════════════════════
        for (int i = 0; i < students.Count; i++)
        {
            var student = students[i];
            int row = headerRow + 1 + i;
            bool isEven = i % 2 == 0;

            var rowBg = isEven
                ? XLColor.FromHtml("#FFFFFF")
                : XLColor.FromHtml("#F0F8FF");

            // قيم الخلايا
            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 2).Value = student.StdCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault().Center.Name ?? "";
            ws.Cell(row, 3).Value = student.Name ?? "";
            ws.Cell(row, 4).Value = student.EnName ?? "";
            ws.Cell(row, 5).Value = student.CivilId ?? "";
            ws.Cell(row, 6).Value = student.Mobile ?? "";
            ws.Cell(row, 7).Value = student.Gender?.Name ?? "";
            ws.Cell(row, 8).Value = student.Level?.Name ?? "";
            ws.Cell(row, 9).Value = student.IsUnrwa ? "نعم" : "";
            ws.Cell(row, 10).Value = student.IsSpecialNeeds ? "نعم" : "";

            // تنسيق الصف كاملاً
            var rowRange = ws.Range(row, 1, row, totalCols);
            rowRange.Style.Fill.BackgroundColor = rowBg;
            rowRange.Style.Font.FontSize  = 11;
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#DDDDDD");
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#EEEEEE");

            // توسيط بعض الأعمدة
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // تلوين خلية الجنس
            if (student.Gender?.Name == "ذكر")
                ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromHtml("#0D6EFD");
            else if (student.Gender?.Name == "أنثى")
                ws.Cell(row, 7).Style.Font.FontColor = XLColor.FromHtml("#DC3545");

            // تلوين أنروا واحتياجات خاصة
            if (student.IsUnrwa)
            {
                ws.Cell(row, 9).Style.Font.Bold = true;
                ws.Cell(row, 9).Style.Font.FontColor = XLColor.FromHtml("#0DCAF0");
                ws.Cell(row, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
            if (student.IsSpecialNeeds)
            {
                ws.Cell(row, 10).Style.Font.Bold = true;
                ws.Cell(row, 10).Style.Font.FontColor = XLColor.FromHtml("#FFC107");
                ws.Cell(row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }
        }

        // ══════════════════════════════════════════════════════
        //  صف الإجماليات
        // ══════════════════════════════════════════════════════
        int totalRow = headerRow + students.Count + 1;

        ws.Range(totalRow, 1, totalRow, 5).Merge();
        ws.Cell(totalRow, 1).Value = $"الإجمالي: {students.Count} طالب";
        ws.Cell(totalRow, 1).Style.Font.Bold = true;
        ws.Cell(totalRow, 1).Style.Font.FontSize = 11;
        ws.Cell(totalRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        var maleCount   = students.Count(s => s.Gender?.Name == "ذكر");
        var femaleCount = students.Count(s => s.Gender?.Name == "أنثى");
        var unrwaCount  = students.Count(s => s.IsUnrwa);
        var specialCount= students.Count(s => s.IsSpecialNeeds);

        ws.Cell(totalRow, 6).Value = $"ذ:{maleCount} | إ:{femaleCount}";
        ws.Cell(totalRow, 6).Style.Font.Bold = true;
        ws.Cell(totalRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        ws.Cell(totalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Range(totalRow, 6, totalRow, 7).Merge();

        ws.Cell(totalRow, 8).Value = unrwaCount.ToString();
        ws.Cell(totalRow, 8).Style.Font.Bold = true;
        ws.Cell(totalRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        ws.Cell(totalRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell(totalRow, 9).Value = specialCount.ToString();
        ws.Cell(totalRow, 9).Style.Font.Bold = true;
        ws.Cell(totalRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        ws.Cell(totalRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Row(totalRow).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        ws.Row(totalRow).Style.Border.OutsideBorderColor = XLColor.FromHtml("#009EDB");
        ws.Row(totalRow).Height = 20;

        // ══════════════════════════════════════════════════════
        //  تجميد الصفوف العلوية عند التمرير
        // ══════════════════════════════════════════════════════
        ws.SheetView.FreezeRows(headerRow);

        // ── تحويل إلى bytes ────────────────────────────────────
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
