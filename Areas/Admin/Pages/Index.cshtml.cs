using KintaiSystem.Models;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KintaiSystem.Areas.Admin.Pages
{
    public class IndexModel : KintaiSystemPageModelBase
    {
        public IndexModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<IndexModel> logger)
            :base(env, configuration, logger)
        {
            Title = "KintaiSystem Admin";
        }
    }
}
