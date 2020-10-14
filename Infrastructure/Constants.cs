namespace KintaiSystem.Infrastructure
{
    public static class Constants
    {
        public const string BearerAuthorizationScheme = "Bearer";

        public static class Scope
        {
            public const string UserRead = "User.Read";
        }

        public static class UserSecrets
        {
            public const string ClientSecret = "LocalAppSettings:ClientSecret";

            public static class ConnectionStrings
            { 
                public const string Database = "local-sqldb";
            }

            public const string SlackAppToken = "LocalAppSettings:SlackAppToken";
        }

        public static class AppSettingPath
        {
            public static class Secrets
            {
                public const string ClientSecret = "AppSettings:Secrets:ClientSecret";
                public const string DbConnectionString = "AppSettings:Secrets:DbConnectionString";
                public const string SlackAppToken = "AppSettings:Secrets:SlackAppToken";
            }
        }

        public static class Session
        {
            public const string AuthenticationCode = "AuthenticationCode";
            public const string AuthUriToken = "AuthenticationCodeRequestToken";
            public const string AccessToken = "AccessToken";

            public const string UserDataJson = "UserDataJson";
            public const string EmployeeId = "EmployeeId";
            public const string SystemRoleId = "SystemRoleId";

            public const string RequestPath = "RequestPath";
        }
    }
}