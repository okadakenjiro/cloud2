using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace KintaiSystem.Areas.Auth.Pages.Aadlogin
{
    public class CallbackModel : PageModel
    {
        private readonly ILogger<CallbackModel> _logger;

        public CallbackModel(ILogger<CallbackModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string token)
        {
            HttpContext.Session.SetString(Infrastructure.Constants.Session.AuthenticationCode, code ?? "unknown code");

            _logger.LogDebug(@"Query parameter token: ""{0}""", token);

            var requestPath = HttpContext.Session.GetString(Infrastructure.Constants.Session.RequestPath);

            return Redirect(requestPath);
        }
    }
}
