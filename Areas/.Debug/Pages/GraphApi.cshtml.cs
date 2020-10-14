using System.Threading.Tasks;

using KintaiSystem.Infrastructure.Json;
using KintaiSystem.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace KintaiSystem.Areas.Debug.Pages
{
    public class GraphApiModel : KintaiSystemPageModelBase
    {
        #region ViewData

        public GraphUserJsonData UserData { get; private set; }
        #endregion

        public GraphApiModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<GraphApiModel> logger)
            : base(env, configuration, logger)
        {
            Title = "KintaiSystem Debugging (Graph API)";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (_env.IsProduction()) return NotFound();

            var me = await GraphClient.Me.Request()
                .Select(u => new
                {
                    u.EmployeeId,
                    u.Surname,
                    u.GivenName,
                    u.Mail
                })
                .GetAsync();

            #region Get UserData
            var userData = new GraphUserJsonData()
            {
                EmployeeId = me.EmployeeId,
                LastName = me.Surname,
                FirstName = me.GivenName,
                MailAddress = me.Mail
            };

            // 画面表示用に格納
            UserData = userData;
            #endregion

            return Page();
        }
    }
}
