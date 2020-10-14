using KintaiSystem.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KintaiSystem.Areas.Home.Pages
{
    public class LogoutModel : KintaiSystemPageModelBase
    {
        public LogoutModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<LogoutModel> logger)
            : base(env, configuration, logger)
        {
            Title = "ログアウトしました";
        }

        public IActionResult OnGet()
        {
            if (!Request.Path.Value.ToLower().Equals("/logout"))
            {
                // /logout（大文字小文字区別しない）以外でのアクセスは404扱い
                return NotFound();
            }

            HttpContext.Session.Clear();

            return Page();
        }
    }
}
