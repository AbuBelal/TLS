// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Services/EmployeeExportService.cs              ║
// ╚══════════════════════════════════════════════════════════════╝

using ClosedXML.Excel;
using SharedLib.DTOs;

namespace APIServerLib.Services;

public static class EmployeeExportService
{
    public static byte[] GenerateExcel(
        List<EmployeeListItemDto> employees,
        string sheetTitle,
        string centerName)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("الموظفون");
        ws.RightToLeft = true;

        const int totalCols = 8;

        // ══ الصف 1: عنوان التقرير ══════════════════════════════
        ws.Range(1, 1, 1, totalCols).Merge();
        var titleCell = ws.Cell(1, 1);
        titleCell.Value = $"قائمة الموظفين — {centerName}";
        titleCell.Style.Font.Bold      = true;
        titleCell.Style.Font.FontSize  = 16;
        titleCell.Style.Font.FontColor = XLColor.White;
        titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#009EDB");
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleCell.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
        ws.Row(1).Height = 30;

        // ══ الصف 2: معلومات التصدير ════════════════════════════
        ws.Range(2, 1, 2, totalCols).Merge();
        var infoCell = ws.Cell(2, 1);
        infoCell.Value = $"{sheetTitle}     |     تاريخ التصدير: {DateTime.Now:dd/MM/yyyy HH:mm}     |     العدد: {employees.Count} موظف";
        infoCell.Style.Font.FontSize  = 10;
        infoCell.Style.Font.Italic    = true;
        infoCell.Style.Font.FontColor = XLColor.FromHtml("#555555");
        infoCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        infoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Row(2).Height = 18;

        // ══ الصف 3: رؤوس الأعمدة ══════════════════════════════
        int headerRow = 3;
        var headers = new (string Label, double Width)[]
        {
            ("#",              5),
            ("الاسم بالعربي", 22),
            ("الاسم بالإنجليزي", 20),
            ("رقم الهوية",    14),
            ("رقم الجوال",    13),
            ("الجنس",          9),
            ("الوظيفة",       14),
            ("التخصص",        16),
            ("المركز",        22),
        };

        for (int c = 0; c < headers.Length; c++)
        {
            var cell = ws.Cell(headerRow, c + 1);
            cell.Value = headers[c].Label;
            cell.Style.Font.Bold      = true;
            cell.Style.Font.FontSize  = 11;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical   = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#005577");
            ws.Column(c + 1).Width = headers[c].Width;
        }
        ws.Row(headerRow).Height = 22;

        // ══ بيانات الموظفين ════════════════════════════════════
        for (int i = 0; i < employees.Count; i++)
        {
            var emp = employees[i];
            int row = headerRow + 1 + i;
            bool isEven = i % 2 == 0;

            var rowBg = isEven
                ? XLColor.FromHtml("#FFFFFF")
                : XLColor.FromHtml("#F0F8FF");

            ws.Cell(row, 1).Value = i + 1;
            ws.Cell(row, 2).Value = emp.Name         ?? "";
            ws.Cell(row, 3).Value = emp.EnName        ?? "";
            ws.Cell(row, 4).Value = emp.CivilId       ?? "";
            ws.Cell(row, 5).Value = emp.Mobile         ?? "";
            ws.Cell(row, 6).Value = emp.GenderName     ?? "";
            ws.Cell(row, 7).Value = emp.JobName        ?? "";
            ws.Cell(row, 8).Value = emp.SpecializationName ?? "";
            ws.Cell(row, 9).Value = emp.CenterName ?? "";

            var rowRange = ws.Range(row, 1, row, totalCols);
            rowRange.Style.Fill.BackgroundColor = rowBg;
            rowRange.Style.Font.FontSize  = 11;
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#DDDDDD");
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#EEEEEE");

            // توسيط الرقم والجنس
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // تلوين الجنس
            if (emp.GenderName == "ذكر")
                ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#0D6EFD");
            else if (emp.GenderName == "أنثى")
                ws.Cell(row, 6).Style.Font.FontColor = XLColor.FromHtml("#DC3545");
        }

        // ══ صف الإجماليات ══════════════════════════════════════
        int totalRow = headerRow + employees.Count + 1;

        ws.Range(totalRow, 1, totalRow, 5).Merge();
        ws.Cell(totalRow, 1).Value = $"الإجمالي: {employees.Count} موظف";
        ws.Cell(totalRow, 1).Style.Font.Bold = true;
        ws.Cell(totalRow, 1).Style.Font.FontSize = 11;
        ws.Cell(totalRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        var maleCount   = employees.Count(e => e.GenderName == "ذكر");
        var femaleCount = employees.Count(e => e.GenderName == "أنثى");

        ws.Range(totalRow, 6, totalRow, 8).Merge();
        ws.Cell(totalRow, 6).Value = $"ذكور: {maleCount}  |  إناث: {femaleCount}";
        ws.Cell(totalRow, 6).Style.Font.Bold = true;
        ws.Cell(totalRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        ws.Cell(totalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Row(totalRow).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        ws.Row(totalRow).Style.Border.OutsideBorderColor = XLColor.FromHtml("#009EDB");
        ws.Row(totalRow).Height = 20;

        // ══ تجميد الرؤوس ═══════════════════════════════════════
        ws.SheetView.FreezeRows(headerRow);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
