using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Fixed
{
    public static class SystemSettings
    {
        public const string DebugBaseUrl = "https://localhost:7075";
        public enum RegionType { Debug,Test, North, WestMiddle, WestGaza }
        // حدد المنطقة هنا فقط
        private const  RegionType CurrentRegion = RegionType.Debug;

        public static string SelectedBaseUrl => CurrentRegion switch
        {
            RegionType.Debug => DebugBaseUrl,
            RegionType.North => "https://manapi.runasp.net/",
            RegionType.Test => "https://tlsapi.runasp.net/",
            RegionType.WestMiddle => "https://midapi.tryasp.net/",
            RegionType.WestGaza => "https://wgazaapi.runasp.net/",
            _ =>DebugBaseUrl
        };

        public static string SelectedAreaAr => CurrentRegion switch
        {
            RegionType.Debug => "التجريب",
            RegionType.North => "الشمال",
            RegionType.WestMiddle => "غرب الوسطى",
            RegionType.WestGaza => "غرب غزة",
            RegionType.Test => "الشمال - اختبار",
            _ => "الشمال",
        };
        public static string SelectedAreaEn => CurrentRegion switch
        {
            RegionType.Debug => "ForDebug",
            RegionType.North => "North",
            RegionType.WestMiddle => "WestMiddle",
            RegionType.WestGaza => "WestGaza",
            RegionType.Test => "ForTest",
            _ => "North",
        };

    }
    public static class AppRoles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string UserViewer = "User_Viewer";
        public const string AdminViewer = "Admin_Viewer";
        public static List<string> GetRoles()
        {
            return new List<string> { Admin, User, UserViewer, AdminViewer };
        }

    }
    public static class LookupTypes
    {
        public const string Gender = "Gender";
        public const string Level = "Level";
        public const string Job = "Job";
        public const string Specialization = "Specialization";
        public const string Governate = "Governate";
        public const string Area = "Area";
        public const string WHoures = "WHoures";
        public const string Mesure = "Mesure";
        public const string Mesure1 = "Mesure1";
        public const string Mesure2 = "Mesure2";
        public const string Mesure3 = "Mesure3";
        public const string Mesure4 = "Mesure4";
        public const string Vacation = "Vacation";

    }
    public static class GlobalData
    {
        // تعريف القاموس كـ static و readonly
            public static readonly Dictionary<string, string> ArabicDays = new Dictionary<string, string>
        {
            { "Saturday", "السبت" },
            { "Sunday", "الأحد" },
            { "Monday", "الاثنين" },
            { "Tuesday", "الثلاثاء" },
            { "Wednesday", "الأربعاء" },
            { "Thursday", "الخميس" },
            { "Friday", "الجمعة" }
        };
    }
}
