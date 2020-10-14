using KintaiSystem.Filters;
using KintaiSystem.Helpers;
using KintaiSystem.Infrastructure;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

namespace KintaiSystem
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;

            if (env.IsDevelopment())
            {
                // 開発：UserSecretsから取得
                Configuration[Constants.AppSettingPath.Secrets.ClientSecret] =
                    configuration.GetValue<string>(Constants.UserSecrets.ClientSecret);

                Configuration[Constants.AppSettingPath.Secrets.DbConnectionString] =
                    configuration.GetConnectionString(Constants.UserSecrets.ConnectionStrings.Database);

                Configuration[Constants.AppSettingPath.Secrets.SlackAppToken] =
                    configuration.GetValue<string>(Constants.UserSecrets.SlackAppToken);
            }
            else
            {
                // 開発以外：appsettings.jsonの値でKeyVaultを参照しシークレットを取得して上書き
                Configuration[Constants.AppSettingPath.Secrets.ClientSecret] =
                    KeyVaultHelper.GetSecretFromKeyVaultAsync(Configuration[Constants.AppSettingPath.Secrets.ClientSecret]).Result.Result;

                Configuration[Constants.AppSettingPath.Secrets.DbConnectionString] =
                    KeyVaultHelper.GetSecretFromKeyVaultAsync(Configuration[Constants.AppSettingPath.Secrets.DbConnectionString]).Result.Result;

                Configuration[Constants.AppSettingPath.Secrets.SlackAppToken] =
                    KeyVaultHelper.GetSecretFromKeyVaultAsync(Configuration[Constants.AppSettingPath.Secrets.SlackAppToken]).Result.Result;
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
                // Handling SameSite cookie according to https://docs.microsoft.com/en-us/aspnet/core/security/samesite?view=aspnetcore-3.1
                options.HandleSameSiteCookieCompatibility();
            });

            services.AddOptions();

            services.AddSession();

            services
                .AddRazorPages(options =>
                {
                    options.Conventions.AddAreaPageRoute("Home", "/Index", "/");
                    options.Conventions.AddAreaPageRoute("Home", "/Logout", "/Logout");
                })
                .AddMvcOptions(options =>
                {
                    options.Filters.Add(typeof(GetUserDataAsyncPageFilter));
                })
                .AddMicrosoftIdentityUI();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // デフォルトのHSTS値は30日です。 運用シナリオではこれを変更することができます。https://aka.ms/aspnetcore-hsts を参照してください。
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAreaControllerRoute(
                    name: "default",
                    areaName: "Home",
                    pattern: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" }
                );
                endpoints.MapAreaControllerRoute(
                    name: "admin_area",
                    areaName: "Admin",
                    pattern: "Admin/{controller}/{action}/{id?}",
                    defaults: new { action = "Index" }
                );
                endpoints.MapAreaControllerRoute(
                    name: "manager_area",
                    areaName: "Manager",
                    pattern: "Manager/{controller}/{action}/{id?}",
                    defaults: new { action = "Index" }
                );
                endpoints.MapAreaControllerRoute(
                    name: "member_area",
                    areaName: "Member",
                    pattern: "Member/{controller}/{action}/{id?}",
                    defaults: new { action = "Index" }
                );

                endpoints.MapRazorPages();
            });

            app.UseCookiePolicy();
            // ↑（Redirectするときのみ）UseCookiePolicyはUseEndpointsより後に記述すること
            // https://ja.stackoverflow.com/questions/51193/asp-net-core-mvc%E3%81%A7redirect%E3%81%99%E3%82%8B%E3%81%A8session%E3%81%AB%E4%BF%9D%E6%8C%81%E3%81%97%E3%81%9F%E5%80%A4%E3%81%8C%E6%B6%88%E3%81%88%E3%82%8B
        }
    }
}
