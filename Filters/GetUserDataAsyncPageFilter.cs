using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using KintaiSystem.DB;
using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;
using KintaiSystem.Infrastructure;
using KintaiSystem.Infrastructure.Json;
using KintaiSystem.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;

using Constants = KintaiSystem.Infrastructure.Constants;

namespace KintaiSystem.Filters
{
    public class GetUserDataAsyncPageFilter : IAsyncPageFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GetUserDataAsyncPageFilter> _logger;

        private readonly AppSettings _appSettings;

        public GetUserDataAsyncPageFilter(IWebHostEnvironment env, IConfiguration configuration, ILogger<GetUserDataAsyncPageFilter> logger)
        {
            _env = env;
            _configuration = configuration;
            _logger = logger;

            _appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            if (!context.ActionDescriptor.AreaName.Equals(".auth"))
            {
                #region ADから情報とる
                var model = new UserDataModel(host: context.HttpContext.Request.Host)
                {
                    TenantId = _appSettings.TenantId,
                    ClientId = _appSettings.ClientId,
                };

                // セッションに保存した認証コードおよびアクセストークンを取得する
                var authenticationCode = context.HttpContext.Session.GetString(Constants.Session.AuthenticationCode);
                var accessToken = context.HttpContext.Session.GetString(Constants.Session.AccessToken);

                if (string.IsNullOrEmpty(authenticationCode))
                {
                    // codeがなければリクエストする
                    _logger.LogDebug(@"Not have authentication code, redirect to request uri.");

                    #region Request Authorization Code
                    // 許可を求めるスコープ
                    var scopes = string.Join(" ", new List<string>()
                    {
                        Constants.Scope.UserRead,
                    });
                    _logger.LogDebug(@"Authentication request scopes: ""{0}""", scopes);

                    // 認証後にリダイレクトされたURLに含まれる'state'クエリの値と比べることで、CSRF対策が行える
                    var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    context.HttpContext.Session.SetString(Constants.Session.AuthUriToken, token);
                    _logger.LogDebug(@"Session saved: Auth-code request token ""{0}""", token);


                    var redirectUri = model.GetRequestAuthenticationCodeUri(scopes, token);


                    _logger.LogDebug(@"Redirect target uri: ""{0}""", redirectUri.AbsoluteUri);

                    // リダイレクト前に、リクエストパスをセッションに格納
                    context.HttpContext.Session.SetString(Constants.Session.RequestPath, context.HttpContext.Request.Path.Value);

                    context.Result = new RedirectResult(redirectUri.AbsoluteUri);
                    return;
                    #endregion
                }
                else if (string.IsNullOrEmpty(accessToken))
                {
                    // codeがある場合はユーザー情報を取得する
                    _logger.LogDebug(@"Having authentication code, get access token.");

                    #region Get AccessToken
                    accessToken = await model.GetAccessTokenAsync(authenticationCode, _appSettings.Secrets.ClientSecret);
                    _logger.LogDebug(@"GetAccessTokenAsync status code: ""{0} {1}""", (int)model.Response.StatusCode, model.Response.StatusCode);
                    _logger.LogDebug(@"Access token: ""{0}""", accessToken);

                    context.HttpContext.Session.SetString(Constants.Session.AccessToken, accessToken ?? "!!!! NO ACCESSTOKEN !!!!");
                    _logger.LogDebug(@"Session saved: Access token ""{0}""", accessToken);
                    #endregion
                }

                _logger.LogDebug(@"Get user data(from MS Graph API).");


                #region Get UserData
                var graphClient = new GraphServiceClient(new DelegateAuthenticationProvider((message) =>
                {
                    message.Headers.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

                    return Task.CompletedTask;
                }));

                var me = await graphClient.Me.Request()
                    .Select(u => new
                    {
                        u.EmployeeId,
                        u.Surname,
                        u.GivenName,
                        u.Mail
                    })
                    .GetAsync();
                var userData = new GraphUserJsonData()
                {
                    EmployeeId = me.EmployeeId,
                    LastName = me.Surname,
                    FirstName = me.GivenName,
                    MailAddress = me.Mail
                };
                #endregion
                #endregion

                #region テーブルから情報取る
                UsersHelper dbHelper = new UsersHelper(_appSettings.Secrets.DbConnectionString);
                UsersData usersTableData = dbHelper.SelectData(userData.EmployeeId);
                #endregion

                #region セッションに情報入れる
                context.HttpContext.Session.SetString(Constants.Session.EmployeeId, usersTableData.EmployeeId);
                context.HttpContext.Session.SetString(Constants.Session.SystemRoleId, usersTableData.SystemRoleId);
                #endregion
            }

            await next.Invoke();
        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            await Task.CompletedTask;
        }
    }
}
