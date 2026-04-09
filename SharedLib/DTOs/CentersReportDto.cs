using System.Collections.Generic;

namespace SharedLib.DTOs;

public class CenterReportDto
{
    public long CenterId { get; set; }
    public string? CenterCode { get; set; }
    public string? CenterName { get; set; }
    public int Rooms { get; set; }
    public int Tarpaulins { get; set; }
    public int OtherSpaces { get; set; }

    public int FirstGradeMales { get; set; }
    public int FirstGradeFemales { get; set; }
    public int SecondGradeMales { get; set; }
    public int SecondGradeFemales { get; set; }
    public int ThirdGradeMales { get; set; }
    public int ThirdGradeFemales { get; set; }
    public int FourthGradeMales { get; set; }
    public int FourthGradeFemales { get; set; }
    public int FifthGradeMales { get; set; }
    public int FifthGradeFemales { get; set; }
    public int SixthGradeMales { get; set; }
    public int SixthGradeFemales { get; set; }
    public int SeventhGradeMales { get; set; }
    public int SeventhGradeFemales { get; set; }
    public int EighthGradeMales { get; set; }
    public int EighthGradeFemales { get; set; }
    public int NinthGradeMales { get; set; }
    public int NinthGradeFemales { get; set; }
    public int TenthGradeMales { get; set; }
    public int TenthGradeFemales { get; set; }
    public int EleventhGradeMales { get; set; }
    public int EleventhGradeFemales { get; set; }
    public int TwelfthGradeMales { get; set; }
    public int TwelfthGradeFemales { get; set; }

    public int UnrwaCount { get; set; }
    public int SpecialNeedsCount { get; set; }
    public int TotalMales { get; set; }
    public int TotalFemales { get; set; }
    public int GrandTotal { get; set; }
}

public class CenterReportResponse
{
    public List<CenterReportDto> Centers { get; set; } = new();
    public List<string> AvailableLevels { get; set; } = new();
}
