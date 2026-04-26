// ╔══════════════════════════════════════════════════════════════╗
// ║  APIServerLib/Services/DailyReportExportService.cs            ║
// ║  يحوّل قائمة التقارير اليومية إلى ملف Excel باستخدام ClosedXML        ║
// ╚══════════════════════════════════════════════════════════════╝

using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using SharedLib.Entities;
using System.Net.NetworkInformation;

namespace APIServerLib.Services;

public static class DailyReportExportService
{
    /// <summary>
    /// يُنشئ ملف Excel من قائمة التقارير اليومية ويُعيده كـ byte[]
    /// </summary>
    public static byte[] GenerateExcel(List<DailyReport> reports, string sheetTitle)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("التقرير اليومي");

        // ── إعداد اتجاه RTL للورقة ─────────────────────────────
        //ws.RightToLeft = true;

        // ══════════════════════════════════════════════════════
        //  الصف الأول: عنوان التقرير (مدمج)
        // ══════════════════════════════════════════════════════
        int totalCols = 37;
        int headerRow = 1;
        var headers = new[]
        {
            ("Date",           18),//1
            ("EC Code",  11),//2
            ("Area", 10),//3
            ("EArea", 10),//4
            ("EC Name", 25),//5
            ("# of CRs", 5),//6
            ("G1B", 5),//7
            ("G1G", 5),//8
            ("G2B", 5),//9
            ("G2G", 5),//10
            ("G3B", 5),//11
            ("G3G", 5),//12
            ("G4B", 5),//13
            ("G4G", 5),//14
            ("G5B", 5),//15
            ("G5G", 5),//16
            ("G6B", 5),//17
            ("G6G", 5),//18
            ("G7B", 5),//19
            ("G7G", 5),//20
            ("G8B", 5),//21
            ("G8G", 5),//22
            ("G9B", 5),//23
            ("G9G", 5),//24
            ("UN Std", 10 ),//25
            ("Disabilities", 10),//26
            ("Student registered", 10),//27
            ("WFP distribuation", 10),//28
            ("WFP loss", 10),//29
            ("WFP Total", 10),//30
            ("Non-UN Std", 10),//31
            ("TotalBoys", 10),//32
            ("TotalGirls", 10),//33
            ("# of Students", 10),//34
           
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
        reports = reports.OrderBy(r => r.Center?.SortOrder).ToList();
        for (int i = 0; i < reports.Count; i++)
        {
            var report = reports[i];
            int row = headerRow + 1 + i;
            bool isEven = i % 2 == 0;

            var rowBg = isEven
                ? XLColor.FromHtml("#FFFFFF")
                : XLColor.FromHtml("#F0F8FF");

            // قيم الخلايا
            ws.Cell(row, 1).Value  = report.ReportDate.ToString("yyyy-MM-dd");
            ws.Cell(row, 2).Value  = report.Center?.CenterCode;
            ws.Cell(row, 3).Value = "";
            ws.Cell(row, 4).Value  = "";
            ws.Cell(row, 5).Value = report.Center?.Name;
            ws.Cell(row, 6).Value  = report.Center?.Rooms + report.Center?.Tarpaulins;
            ws.Cell(row, 7).Value = report.AttMale01;
            ws.Cell(row, 8).Value = report.AttFemale01;
            ws.Cell(row, 9).Value = report.AttMale02;
            ws.Cell(row, 10).Value = report.AttFemale02;
            ws.Cell(row, 11).Value = report.AttMale03;
            ws.Cell(row, 12).Value = report.AttFemale03;
            ws.Cell(row, 13).Value = report.AttMale04;
            ws.Cell(row, 14).Value = report.AttFemale04;
            ws.Cell(row, 15).Value = report.AttMale05;
            ws.Cell(row, 16).Value = report.AttFemale05;
            ws.Cell(row, 17).Value = report.AttMale06;
            ws.Cell(row, 18).Value = report.AttFemale06;
            ws.Cell(row, 19).Value = report.AttMale07;
            ws.Cell(row, 20).Value = report.AttFemale07;
            ws.Cell(row, 21).Value = report.AttMale08;
            ws.Cell(row, 22).Value = report.AttFemale08;
            ws.Cell(row, 23).Value = report.AttMale09;
            ws.Cell(row, 24).Value = report.AttFemale09;
            ws.Cell(row, 25).Value = report.IsUNRWA;
            ws.Cell(row, 26).Value = report.Disabilities;
            ws.Cell(row, 27).Value = report.RegTotal;
            ws.Cell(row, 28).Value = report.WFPBiscDist;
            ws.Cell(row, 29).Value = report.WFPBiscLost;
            ws.Cell(row, 30).Value = report.WFPBiscTotal;
            ws.Cell(row, 31).Value = report.IsNotUNRWA;
            ws.Cell(row, 32).Value = report.AttMaleTotal;
            ws.Cell(row, 33).Value = report.AttFemaleTotal;
            ws.Cell(row, 34).Value = report.AttTotal;

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

        }

        // ══════════════════════════════════════════════════════
        //  صف الإجماليات
        // ══════════════════════════════════════════════════════
        //int totalRow = headerRow + students.Count + 1;

        //ws.Range(totalRow, 1, totalRow, 5).Merge();
        //ws.Cell(totalRow, 1).Value = $"الإجمالي: {students.Count} طالب";
        //ws.Cell(totalRow, 1).Style.Font.Bold = true;
        //ws.Cell(totalRow, 1).Style.Font.FontSize = 11;
        //ws.Cell(totalRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
        //ws.Cell(totalRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        //var maleCount   = students.Count(s => s.Gender?.Name == "ذكر");
        //var femaleCount = students.Count(s => s.Gender?.Name == "أنثى");
        //var unrwaCount  = students.Count(s => s.IsUnrwa);
        //var specialCount= students.Count(s => s.IsSpecialNeeds);

        //ws.Cell(totalRow, 6).Value = $"ذ:{maleCount} | إ:{femaleCount}";
        //ws.Cell(totalRow, 6).Style.Font.Bold = true;
        //ws.Cell(totalRow, 6).Style.Fill.BackgroundColor = XLColor.FromHtml("#E8F4FD");
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
