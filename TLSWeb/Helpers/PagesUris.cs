namespace TLSWeb.Helpers
{
    public static class PagesUris
    {
        public const string Home = "/";
        public static class Auth
        {
            public const string PriorUrl = "";
            public const string Login = PriorUrl+ "/login";
            public const string Register = PriorUrl+ "/register";
        }
        public static class UsersPages
        {
            public const string PriorUrl = "Users";
            public const string Manage = PriorUrl+ "/Manage";
            public const string Add = PriorUrl+ "/Add";
            public const string Edit = PriorUrl+ "/Edit";
            public const string Profile = PriorUrl+ "/profile";
        }

        public static class EmployeesPages
        {
            public const string PriorUrl = "Employees";
            public const string Manage = PriorUrl + "/Manage";
            public const string Add = PriorUrl + "/Add";
            public const string Edit = PriorUrl + "/Edit";
        }
        public static class StudentsPages
        {
            public const string PriorUrl = "Students";
            public const string Manage = PriorUrl + "/Manage";
            public const string Add = PriorUrl + "/Add";
            public const string Edit = PriorUrl + "/Edit";
        }
        public static class LookupsValuePages
        {
            public const string PriorUrl = "Lookups";
            public const string Manage = PriorUrl + "/Manage";
            public const string Add = PriorUrl + "/Add";
            public const string Edit = PriorUrl + "/Edit";
        }
        public static class Admin
        {
            public const string PriorUrl = "Admin";
            public const string DashBoard = PriorUrl + "/dashboard";
            public const string EditCenters = PriorUrl + "/EditCenters";
            public const string ResetPassword = PriorUrl + "/ResetPassword";

        }

        public static class CenterPages
        {
            public const string PriorUrl = "Center";
            public const string Edit = PriorUrl + "/Edit";
        }
    }
}
