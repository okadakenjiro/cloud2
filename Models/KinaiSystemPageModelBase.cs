using System.Data;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using KintaiSystem.DB;
using KintaiSystem.Helpers;
using KintaiSystem.Infrastructure;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

namespace KintaiSystem.Models
{
    public class KintaiSystemPageModelBase : PageModel
    {
        [ViewData]
        public string Title { get; protected set; } = "KintaiSystem";

        protected readonly IWebHostEnvironment _env;
        protected readonly ILogger<PageModel> _logger;

        protected readonly AppSettings _appSettings;

        public GraphServiceClient GraphClient { get; private set; }

        protected KintaiSystemPageModelBase(IWebHostEnvironment env, IConfiguration configuration, ILogger<PageModel> logger)
        {
            _env = env;
            _logger = logger;
            _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();

            GraphClient = new GraphServiceClient(new DelegateAuthenticationProvider((message) =>
            {
                message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, HttpContext.Session.GetString(Infrastructure.Constants.Session.AccessToken));

                return Task.CompletedTask;
            }));
        }

        public bool IsPermitted(string scopeName)
        {
            var systemRoleId = HttpContext.Session.GetString(Infrastructure.Constants.Session.SystemRoleId);

            PermittedScopesHelper dbHelper = new PermittedScopesHelper(_appSettings.Secrets.DbConnectionString);

            return dbHelper.IsExistsData(systemRoleId, scopeName);
        }
    }
}