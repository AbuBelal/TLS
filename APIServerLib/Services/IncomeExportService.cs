using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using SharedLib.DTOs;
using SharedLib.Entities;
using System.Net.NetworkInformation;

namespace APIServerLib.Services;

public static class IncomeReportExportService
{
    /// <summary>
    /// يُنشئ ملف Excel من قائمة التقارير اليومية ويُعيده كـ byte[]
    /// </summary>
    public static byte[] GenerateExcel(List<InCome> reports, string sheetTitle)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("تقرير الوارد");

        // ── إعداد اتجاه RTL للورقة ─────────────────────────────
        ws.RightToLeft = true;

        // ══════════════════════════════════════════════════════
        //  الصف الأول: عنوان التقرير (مدمج)
        // ══════════════════════════════════════════════════════
        int totalCols = 7;
        int headerRow = 1;
        var headers = new[]
        {
            ("#",           5),//1
            ("المركز", 20),//4
            ("الصنف", 20),//4
            ("التاريخ", 15),//3
            ("الكمية",  11),//2
            ("المستلم", 20),//5
            ("ملاحظات", 40),//5
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
        //  بيانات التقرير
        // ══════════════════════════════════════════════════════
        reports = reports.OrderBy(r => r.Center.SortOrder).ThenBy(r=>r.Date).ToList();
        for (int i = 0; i < reports.Count; i++)
        {
            var report = reports[i];
            int row = headerRow + 1 + i;
            bool isEven = i % 2 == 0;

            var rowBg = isEven
                ? XLColor.FromHtml("#FFFFFF")
                : XLColor.FromHtml("#F0F8FF");

            // قيم الخلايا
            ws.Cell(row, 1).Value = (i+1).ToString();
            ws.Cell(row, 2).Value = report.Center.Name;
            ws.Cell(row, 3).Value = report.Name;
            ws.Cell(row, 4).Value = report.Date.ToString("dd/MM/yyyy");
            ws.Cell(row, 5).Value = report.Qnty;
            ws.Cell(row, 6).Value = report.RecipientName;
            ws.Cell(row, 7).Value = report.Comments;
            

            // تنسيق الصف كاملاً
            var rowRange = ws.Range(row, 1, row, totalCols);
            rowRange.Style.Fill.BackgroundColor = rowBg;
            rowRange.Style.Font.FontSize = 11;
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#DDDDDD");
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#EEEEEE");
        }

        // ══════════════════════════════════════════════════════
        //  تجميد الصفوف العلوية عند التمرير
        // ══════════════════════════════════════════════════════
        ws.SheetView.FreezeRows(headerRow);

        // ── تحويل إلى bytes ────────────────────────────────────
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
    public static byte[] GenerateExcelSum(List<IncomeReportDto> reports, string sheetTitle)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("التقرير اليومي");

        // ── إعداد اتجاه RTL للورقة ─────────────────────────────
        ws.RightToLeft = true;

        // ══════════════════════════════════════════════════════
        //  الصف الأول: عنوان التقرير (مدمج)
        // ══════════════════════════════════════════════════════
        int totalCols = 6;
        int headerRow = 1;
        var headers = new[]
        {
            ("#",           5),//1
            ("رقم المبنى",  11),//2
            ("أسماء المراكز", 20),//3
            ("الوارد", 10),//4
            ("الموزع", 10),//5
            ("الرصيد", 10),//6
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
        //  بيانات التقرير
        // ══════════════════════════════════════════════════════
        reports = reports.OrderBy(r => r.CenterSortOrder).DistinctBy(r=>r.CenterBuildingCode).ToList();
        for (int i = 0; i < reports.Count; i++)
        {
            var report = reports[i];
            int row = headerRow + 1 + i;
            bool isEven = i % 2 == 0;

            var rowBg = isEven
                ? XLColor.FromHtml("#FFFFFF")
                : XLColor.FromHtml("#F0F8FF");

            // قيم الخلايا
            ws.Cell(row, 1).Value = (i+1).ToString();
            ws.Cell(row, 2).Value = report.CenterBuildingCode;
            ws.Cell(row, 3).Value = report.CenterNames;
            ws.Cell(row, 4).Value = report.TotalReceived;
            ws.Cell(row, 5).Value = report.TotalDist;
            ws.Cell(row, 6).Value = report.TotalBalance;
            

            // تنسيق الصف كاملاً
            var rowRange = ws.Range(row, 1, row, totalCols);
            rowRange.Style.Fill.BackgroundColor = rowBg;
            rowRange.Style.Font.FontSize = 11;
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#DDDDDD");
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#EEEEEE");

            // توسيط بعض الأعمدة
            //ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            //ws.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        }

        // ══════════════════════════════════════════════════════
        //  صف الإجماليات
        // ══════════════════════════════════════════════════════
        int totalRow = headerRow + reports.Count + 1;

        ws.Range(totalRow, 1, totalRow, 3).Merge();
        //ws.Cell(totalRow, 1).Value = $"الإجمالي: {students.Count} طالب";
        //ws.Cell(totalRow, 1).Style.Font.Bold = true;
        //ws.Cell(totalRow, 1).Style.Font.FontSize = 11;
        //ws.Cell(totalRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        //ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        //var maleCount   = students.Count(s => s.Gender?.Name == "ذكر");
        //var femaleCount = students.Count(s => s.Gender?.Name == "أنثى");
        //var unrwaCount  = students.Count(s => s.IsUnrwa);
        //var specialCount= students.Count(s => s.IsSpecialNeeds);

        ws.Cell(totalRow, 4).Value = reports.Sum(x => x.TotalReceived);
        ws.Cell(totalRow, 4).Style.Font.Bold = true;
        ws.Cell(totalRow, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
        ws.Cell(totalRow, 4).Style.Font.FontColor = XLColor.White;
        //ws.Cell(totalRow, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell(totalRow, 5).Value = reports.Sum(x => x.TotalDist);
        ws.Cell(totalRow, 5).Style.Font.Bold = true;
        ws.Cell(totalRow, 5).Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
        ws.Cell(totalRow, 5).Style.Font.FontColor = XLColor.White;
        //ws.Cell(totalRow, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell(totalRow, 6).Value = reports.Sum(x => x.TotalReceived)- reports.Sum(x => x.TotalDist);
        ws.Cell(totalRow, 6).Style.Font.Bold = true;
        ws.Cell(totalRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
        ws.Cell(totalRow, 6).Style.Font.FontColor = XLColor.White;
        //ws.Cell(totalRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;


        //ws.Range(totalRow, 6, totalRow, 7).Merge();

        //ws.Cell(totalRow, 8).Value = unrwaCount.ToString();
        //ws.Cell(totalRow, 8).Style.Font.Bold = true;
        //ws.Cell(totalRow, 8).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        //ws.Cell(totalRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //ws.Cell(totalRow, 9).Value = specialCount.ToString();
        //ws.Cell(totalRow, 9).Style.Font.Bold = true;
        //ws.Cell(totalRow, 9).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        //ws.Cell(totalRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        //ws.Row(totalRow).Style.Border.OutsideBorder = XLBorderStyleValues.Medium;
        //ws.Row(totalRow).Style.Border.OutsideBorderColor = XLColor.FromHtml("#009EDB");
        //ws.Row(totalRow).Height = 20;

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
