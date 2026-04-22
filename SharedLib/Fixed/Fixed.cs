using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Fixed
{
    public static class SystemSettings
    {
        public const string DebugBaseUrl = "https://localhost:7075";

        #region North
        public const string NorthBaseUrl = "https://manapi.runasp.net/";
        //public const string NorthBaseUrl = "https://tlsapi.runasp.net/";
        public const string NorthAreaAr = "الشمال";
            public const string NorthAreaEn = "North";
        #endregion

        #region WestMiddle
            public const string WestMiddleBaseUrl = "https://midapi.tryasp.net/";
            public const string WestMiddleAreaAr  = "غرب الوسطى";
            public const string WestMiddleAreaEn  = "WestMiddle";
        #endregion

        #region Selected
        public const string SelectedBaseUrl     =  NorthBaseUrl  ;
        public const string SelectedAreaAr      =  NorthAreaAr    ;
        public const string SelectedAreaEn      = NorthAreaEn;
        #endregion
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
}
