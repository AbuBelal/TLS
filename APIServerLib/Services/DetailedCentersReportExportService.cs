// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Services/DetailedCentersReportExportService.cs ║
// ╚══════════════════════════════════════════════════════════════╝

using ClosedXML.Excel;
using SharedLib.DTOs;

namespace APIServerLib.Services;

public static class DetailedCentersReportExportService
{
    public static byte[] GenerateExcel(DetailedCentersReportDto report)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("تقرير المراكز المفصل");

        ws.RightToLeft = true;

        int row = 1;

        // عنوان التقرير
        ws.Range(row, 1, row, 10).Merge();
        var titleCell = ws.Cell(row, 1);
        titleCell.Value = "تقرير مفصل لبيانات المراكز";
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 16;
        titleCell.Style.Font.FontColor = XLColor.White;
        titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#009EDB");
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Row(row).Height = 30;
        row++;

        // معلومات التقرير
        ws.Range(row, 1, row, 10).Merge();
        var infoCell = ws.Cell(row, 1);
        infoCell.Value = $"تاريخ التصدير: {DateTime.Now:dd/MM/yyyy HH:mm}     |     عدد المراكز: {report.Centers.Count}";
        infoCell.Style.Font.FontSize = 10;
        infoCell.Style.Font.Italic = true;
        infoCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        infoCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        ws.Row(row).Height = 18;
        row++;

        // رؤوس الجدول
        var headers = new List<string> { "م", "كود المركز", "اسم المركز" };

        var levelNames = report.LevelMaleTotals.Keys.ToList();
        foreach (var level in levelNames)
        {
            headers.Add($"{level} ذكور");
            headers.Add($"{level} إناث");
        }

        headers.AddRange(new[] { "UNRWA", "احتياجات خاصة", "مجموع الذكور", "مجموع الإناث", "المجموع الكلي" });

        for (int i = 0; i < headers.Count; i++)
        {
            var cell = ws.Cell(row, i + 1);
            cell.Value = headers[i];
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#D4EDDA");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }
        ws.Row(row).Height = 25;
        row++;

        // بيانات المراكز
        for (int i = 0; i < report.Centers.Count; i++)
        {
            var center = report.Centers[i];
            int col = 1;

            ws.Cell(row, col++).Value = i + 1;
            ws.Cell(row, col++).Value = center.CenterCode ?? "";
            ws.Cell(row, col++).Value = center.CenterName;

            foreach (var level in levelNames)
            {
                ws.Cell(row, col++).Value = center.LevelMales[level];
                ws.Cell(row, col++).Value = center.LevelFemales[level];
            }

            ws.Cell(row, col++).Value = center.UnrwaCount;
            ws.Cell(row, col++).Value = center.SpecialNeedsCount;
            ws.Cell(row, col++).Value = center.TotalMales;
            ws.Cell(row, col++).Value = center.TotalFemales;
            ws.Cell(row, col++).Value = center.TotalStudents;

            row++;
        }

        // صف المجاميع
        ws.Cell(row, 1).Value = "المجموع الكلي";
        ws.Range(row, 1, row, 3).Merge();
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");
        ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        int totalCol = 4;
        foreach (var level in levelNames)
        {
            ws.Cell(row, totalCol++).Value = report.LevelMaleTotals[level];
            ws.Cell(row, totalCol++).Value = report.LevelFemaleTotals[level];
        }

        ws.Cell(row, totalCol++).Value = report.GrandTotalUnrwa;
        ws.Cell(row, totalCol++).Value = report.GrandTotalSpecialNeeds;
        ws.Cell(row, totalCol++).Value = report.GrandTotalMales;
        ws.Cell(row, totalCol++).Value = report.GrandTotalFemales;
        ws.Cell(row, totalCol++).Value = report.GrandTotalStudents;

        ws.Row(row).Style.Font.Bold = true;
        ws.Row(row).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD");

        // تنسيق العمود الأول (الأرقام)
        ws.Column(1).Width = 8;
        ws.Column(2).Width = 15;
        ws.Column(3).Width = 25;

        // عرض الأعمدة الأخرى
        for (int c = 4; c <= headers.Count; c++)
        {
            ws.Column(c).Width = 12;
        }

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}