using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Fixed
{
    public static class SystemSettings
    {
        public const string DebugBaseUrl = "https://localhost:7075";

        //#region Test
        //public const string TestBaseUrl = "https://tlsapi.runasp.net/";
        //public const string TestAreaAr     = "الشمال - اختبار";
        //public const string TestAreaEn     = "ForTest";
        //#endregion

        //#region North
        //public const string NorthBaseUrl = "https://manapi.runasp.net/";
        //public const string NorthAreaAr     = "الشمال";
        //public const string NorthAreaEn     = "North";
        //#endregion

        //#region WestMiddle
        //    public const string WestMiddleBaseUrl = "https://midapi.tryasp.net/";
        //    public const string WestMiddleAreaAr  = "غرب الوسطى";
        //    public const string WestMiddleAreaEn  = "WestMiddle";
        //#endregion

        //#region Selected
        //public const string SelectedBaseUrl     =  WestMiddleBaseUrl   ;
        //public const string SelectedAreaAr      =  WestMiddleAreaAr    ;
        //public const string SelectedAreaEn = WestMiddleAreaEn;
        //#endregion
        public enum RegionType { Debug,Test, North, WestMiddle }
        // حدد المنطقة هنا فقط
        private const  RegionType CurrentRegion = RegionType.North;

        public static string SelectedBaseUrl => CurrentRegion switch
        {
            RegionType.Debug => DebugBaseUrl,
            RegionType.North => "https://manapi.runasp.net/",
            RegionType.Test => "https://tlsapi.runasp.net/",
            RegionType.WestMiddle => "https://midapi.tryasp.net/",
            _ =>DebugBaseUrl
        };

        public static string SelectedAreaAr => CurrentRegion switch
        {
            RegionType.Debug => "التجريب",
            RegionType.North => "الشمال",
            RegionType.WestMiddle => "غرب الوسطى",
            RegionType.Test => "الشمال - اختبار",
            _ => "الشمال",
        };
        public static string SelectedAreaEn => CurrentRegion switch
        {
            RegionType.Debug => "North",
            RegionType.North => "North",
            RegionType.WestMiddle => "WestMiddle",
            RegionType.Test => "ForTest",
            _ => "North",
        };

    }
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string Viewer = "Viewer";

    }
    public static class LookupTypes
    {
        public const string Gender = "Gender";
        public const string Level = "Level";
        public const string Job = "Job";
        public const string Specialization = "Specialization";
        public const string WHoures = "WHoures";

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
