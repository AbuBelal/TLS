using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace TLSClientSharedLib.Helpers
{
    public static class ApiUrls
    {
        //public const string BaseUrl = "https://manapi.runasp.net/";    // "https://localhost:7075";


        
#if DEBUG
        //public const string BaseUrl = "https://localhost:7075";
        public const string BaseUrl = SharedLib.Fixed.SystemSettings.DebugBaseUrl;
#else

     //public const string BaseUrl = "https://manapi.runasp.net/";
     //public const string BaseUrl = "https://tlsapi.runasp.net/";
     //public const string BaseUrl =  "https://midapi.tryasp.net/";
     public const string BaseUrl =SharedLib.Fixed.SystemSettings.SelectedBaseUrl;
#endif
        public static class Auth
        {
                //post ///register
                //post ///login
                //post ///refresh
                //get ///confirmEmail
                //post ///resendConfirmationEmail
                //post ///forgotPassword
                //post ///resetPassword
                //post ///manage/2fa
                //get ///manage/info
                //post ///manage/info
            public const string PriorUrl = "";
            public const string Login = PriorUrl + "/login?useCookies=true";
            public const string Register = PriorUrl + "/Register";
            public const string refresh = PriorUrl + "/refresh";
            public const string forgotPassword = PriorUrl + "/forgotPassword";
            public const string resetPassword = PriorUrl + "/resetPassword";
            public const string UserInfo = PriorUrl + "/manage/info";
            public const string Logout = PriorUrl + "/auth/logout";
        }
        public static class DashBoard
        {
            public const string PriorUrl = "/api/Dashboard";
            public const string Get = PriorUrl;
        }
            public static class Employee
        {
            public const string PriorUrl = "/api/Employee";
            public const string GetAll = PriorUrl;
            public const string GetById = PriorUrl+"/{id}";
            public const string GetByCivilId = PriorUrl+ "/GetByCivilId/{CivilId}";
            public const string GetByEmpId = PriorUrl+ "/GetByEmpId/{EmpId}";
            public const string Insert = PriorUrl;
            public const string AddWithCenter = PriorUrl + "/AddWithCenter";
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
            //    /api/Employee/EmployeeCenterCount/{CenterId}
            public const string EmployeesCenterCount = PriorUrl + "/EmployeeCenterCount/{id}";
            public const string Paginated = PriorUrl + "/paginated";
            public const string ExportFiltered = PriorUrl + "/export/filtered";   
            public const string ExportAll = PriorUrl + "/export/all";         
            public const string Managers = PriorUrl + "/Managers";         
            public const string CenterManager = PriorUrl + "/CenterManager";         
            public const string IsCivilIdDuplicate = PriorUrl + "/IsCivilIdDuplicate";         
            public const string IsEmpIdDuplicate = PriorUrl + "/IsEmpIdDuplicate";         
        }
        public static class Student
        {
            public const string PriorUrl = "/api/Student";
            public const string GetAll = PriorUrl;
            public const string GetById = PriorUrl + "/{id}";
            public const string Insert = PriorUrl;
            public const string AddWithCenter = PriorUrl+ "/AddWithCenter";
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
            //   /api/Student/StudentCenterCount/{CenterId})
            public const string StudentsCenterCount = PriorUrl + "/StudentCenterCount/{id}";
            public const string paginated = PriorUrl + "/paginated";
            public const string ExportFiltered = PriorUrl + "/export/filtered";
            public const string ExportAll = PriorUrl + "/export/all";
        }
        public static class Center
        {
            public const string PriorUrl = "/api/Center";
            public const string GetAll = PriorUrl;
            public const string GetById = PriorUrl + "/{id}";
            public const string Insert = PriorUrl;
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
            public const string MyCenter = PriorUrl + "/my-center";
           

        }
        public static class Reports
        {
            public const string PriorUrl = "/api/DailyReport";
            public const string DailyReport = PriorUrl;
            public const string DailyReportForDate = PriorUrl + "/for-date";
            public const string UpdateDailyReport = PriorUrl ;
            public const string ExportDailyReport = PriorUrl+"/export" ;
            public const string GetBuildingTotalDist = PriorUrl + "/building-total-dist";
        }

        public static class EmpCenter
        {
            public const string PriorUrl = "/api/EmpCenter";
            public const string GetAll = PriorUrl;
            public const string GetById = PriorUrl + "/{id}";
            public const string Insert = PriorUrl;
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
        }

        public static class StdCenter
        {
            public const string PriorUrl = "/api/StdCenter";
            public const string GetAll = PriorUrl;
            public const string GetById = PriorUrl + "/{id}";
            public const string Insert = PriorUrl;
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
        }
        public static class LookupValue
        {
            /// <summary>
            ///    /api/LookupValue/type/{typeName}
            /// </summary>
            public const string PriorUrl = "/api/LookupValue";
            public const string GetAll = PriorUrl;
            public const string GetByValueType = PriorUrl+"/type/{TypeName}";
            public const string GetById = PriorUrl + "/{id}";
            public const string Insert = PriorUrl;
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
        }

        public static class User
        {
            public const string PriorUrl = "/api/User";
            public const string GetAll = PriorUrl;
            public const string GetAllWithRoles = PriorUrl+ "/GetAllWithRoles";
            public const string GetUserRole = PriorUrl+ "/GetUserRoles/{id}";
            public const string GetCurUserRole = PriorUrl+ "/GetCurUserRoles";
            public const string Profile = PriorUrl + "/profile";
            public const string Logout = PriorUrl + "/logout";
            public const string LogIn = PriorUrl + "/Login";
            public const string GetById = PriorUrl + "/{id}";
            public const string Insert = PriorUrl + "/AddUser";
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
            //  /api/User/GetByEmail/{Email}
            public const string GetByEmail = PriorUrl + "/GetByEmail/{Email}";
            public const string ChangePassword = PriorUrl + "/ChangePassword";
            public const string ResetPassword = PriorUrl + "/ResetPassword";
        }

        public static class Roles
        {
            //  /api/Roles
            public const string PriorUrl = "/api/Roles";
            public const string GetAll = PriorUrl;
            public const string Insert = PriorUrl;
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
        }
        public static class AdminDashBoard
        {
            public const string PriorUrl = "/api/AdminDashboard";
            public const string Get = PriorUrl;
            public const string DetailedReport = PriorUrl + "/detailed-report";
            public const string ExportDetailedReport = PriorUrl + "/export/detailed-report";
            public const string DailyReport = PriorUrl + "/daily-report";
            public const string LockDailyReport = PriorUrl + "/lock-daily-report";
        }

        public static class AuditLog
        {
            public const string PriorUrl = "/api/AuditLog";
            public const string GetAll = PriorUrl;
            public const string GetByUserId = PriorUrl + "/user/{userId}";
            public const string GetByEntityType = PriorUrl + "/entity/{entityType}";
            public const string Paginated = PriorUrl + "/paginated";
        }

        public static class InCome
        {
            public const string PriorUrl = "/api/InCome";
            public const string GetAll = PriorUrl;
            public const string GetById = PriorUrl + "/{id}";
            public const string GetByCenter = PriorUrl + "/by-center/{centerId}";
            public const string GetByDateRange = PriorUrl + "/by-date";
            public const string GetTotal = PriorUrl + "/total";
            public const string Insert = PriorUrl;
            public const string Update = PriorUrl;
            public const string DeleteById = PriorUrl + "/{id}";
            public const string GetBuildingTotal = PriorUrl + "/Buildingtotal";
        }
        
    }
}
