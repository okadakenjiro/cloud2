namespace KintaiSystem.Infrastructure
{
    public class AppSettings
    {
        public string TenantId { get; set; }

        public string ClientId { get; set; }

        public SecretInfo Secrets { get; set; }

        public class SecretInfo
        {
            public string ClientSecret { get; set; }

            public string DbConnectionString { get; set; }

            public string SlackAppToken { get; set; }
        }
    }
}
