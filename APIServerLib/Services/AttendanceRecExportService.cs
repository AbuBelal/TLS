using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using SharedLib.DTOs;
using SharedLib.Entities;

namespace APIServerLib.Services;

public static class AttendanceRecExportService
{
    public static byte[] GenerateExcel(List<AttendanceRecord> AttendanceRecList,List<LookupValue> LookupList,string sheetTitle,int MonthDaysNo)
    {
        using var wb = new XLWorkbook();
        var ws = wb.AddWorksheet("Staff");
       // ws.RightToLeft = true;

        const int totalCols = 9;

        int headerRow = 1;
        var headers = new (string Label, double Width)[]
        {
         ("Emp No",             10),
         ("Emp Name",           40),
         ("ID Number",            10),
         ("Original Position Title",             15),
         ("Title During Emergency",              10),
         ("New Title During Emergency",              20),
         ("Work Place",              35),
         ("Duty Area",           10),

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
        //Dayes
        for (int D =1; D <= MonthDaysNo; D++)
        {
            var cell = ws.Cell(headerRow, D+headers.Count());
            cell.Value = D.ToString();
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#005577");
            ws.Column(D + headers.Count()).Width = 5;
        }
        //Days No
        var Lastcell = ws.Cell(headerRow, MonthDaysNo+headers.Count()+1);
        Lastcell.Value = "No. of days";
        Lastcell.Style.Font.Bold = true;
        Lastcell.Style.Font.FontSize = 11;
        Lastcell.Style.Font.FontColor = XLColor.White;
        Lastcell.Style.Fill.BackgroundColor = XLColor.FromHtml("#00658E");
        Lastcell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        Lastcell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        Lastcell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        Lastcell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#005577");
        ws.Column(MonthDaysNo + headers.Count() + 1 + 1).Width = 15;

        ws.Row(headerRow).Height = 22;
        bool IsNotReigInACenter = false;


        // ══ بيانات الحضور ════════════════════════════════════
        for (int i = 0; i < AttendanceRecList.Count; i++)
        {
            var emp = AttendanceRecList[i].Employee;
            int row = headerRow + 1 + i;
            //بيانات الموظف
            int c = 1;
            ws.Cell(row, c++).Value = emp?.EmpId;
            ws.Cell(row, c++).Value = emp?.EnName;
            ws.Cell(row, c++).Value = emp?.CivilId;
            ws.Cell(row, c++).Value = emp?.OrgJob?.EnName;
            ws.Cell(row, c++).Value = "";
            ws.Cell(row, c++).Value = "";
            ws.Cell(row, c++).Value = AttendanceRecList[i].Center?.EnName;
            ws.Cell(row, c++).Value = "";



           bool isEven = i % 2 == 0;

            var rowBg = isEven
                ? XLColor.FromHtml("#FFFFFF")
                : XLColor.FromHtml("#F0F8FF");

            int currentHeadersCount = headers.Count();
            var itemType = AttendanceRecList[i].GetType();
            int XCount = 0;
            for (int Col = currentHeadersCount + 1; Col < MonthDaysNo + currentHeadersCount + 1; Col++)
            {
                int dayIndex = Col - currentHeadersCount;

                string IsAttName = $"Day{dayIndex:D2}_IsAttendant";
                string DescName = $"Day{dayIndex:D2}_Desc";

                var IsAttpropertyInfo = itemType.GetProperty(IsAttName);
                var DescpropertyInfo = itemType.GetProperty(DescName);

                if (IsAttpropertyInfo != null)
                {
                    var IsAttValue = IsAttpropertyInfo.GetValue(AttendanceRecList[i]);

                    if (IsAttValue != null && (bool)IsAttValue == true)
                    {
                        ws.Cell(row, Col).Value = "X";
                        XCount++;
                    }
                    else
                        if (DescpropertyInfo != null)
                        {

                            var DescValue = DescpropertyInfo.GetValue(AttendanceRecList[i]);


                            if (DescValue != null)
                            {
                                long descValueLong = (long)DescValue;
                                string VacChar= LookupList.FirstOrDefault(l => l.Id == descValueLong)?.EnName ?? "";
                                ws.Cell(row, Col).Value = VacChar;
                                //switch (VacChar)
                                //{
                                //    case "W": ws.Cell(row, Col).Style.Fill.BackgroundColor = XLColor.FromHtml("#F8F9FA"); break;
                                //    case "H": ws.Cell(row, Col).Style.Fill.BackgroundColor = XLColor.FromHtml("#CFE2FF"); break;
                                //    case "A": ws.Cell(row, Col).Style.Fill.BackgroundColor = XLColor.FromHtml("#D1E7DD"); break;
                                //    case "V": ws.Cell(row, Col).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFF3CD"); break;
                                //    case "M": ws.Cell(row, Col).Style.Fill.BackgroundColor = XLColor.FromHtml("#E2D9F3"); break;
                                //default:
                                //        break;
                                //}
                               
                            }
                        }
                    ws.Cell(row, Col).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    ws.Cell(row, Col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                }
            }

            var TotalCell = ws.Cell(row, MonthDaysNo + headers.Count() + 1);
            TotalCell.Value = XCount;
            TotalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            TotalCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            var rowRange = ws.Range(row, 1, row, MonthDaysNo + currentHeadersCount + 1);
            rowRange.Style.Fill.BackgroundColor = rowBg;
            rowRange.Style.Font.FontSize  = 11;
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#DDDDDD");
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#EEEEEE");

           
        }

        // ══ صف الإجماليات ══════════════════════════════════════
        int totalRow = headerRow + AttendanceRecList.Count + 1;

        // ══ تجميد الرؤوس ═══════════════════════════════════════
        ws.SheetView.FreezeRows(headerRow);

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }   

}
