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
        }
    }
}
