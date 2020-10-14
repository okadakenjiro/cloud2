using KintaiSystem.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KintaiSystem.Areas.Home.Pages
{
    public class PrivacyModel : KintaiSystemPageModelBase
    {
        public PrivacyModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<PrivacyModel> logger)
            : base(env, configuration, logger)
        {
            //Title = "Privacy Policy";
        }
    }
}
