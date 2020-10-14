using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient;

using KintaiSystem.Infrastructure.Json;
using KintaiSystem.Models;
using KintaiSystem.DB;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KintaiSystem.Areas.Admin.Pages.User
{
    /// <summary>
    /// TODO: コメントの整理
    /// </summary>
    public class RegisterModel : KintaiSystemPageModelBase
    {
        #region ViewData
        public GraphUserJsonData UserData { get; set; } = new GraphUserJsonData();

        public List<SelectListItem> SystemRoles { get; set; } = new List<SelectListItem>();

        public string SystemRoleValue { get; set; }
        #endregion

        public RegisterModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<RegisterModel> logger)
            : base(env, configuration, logger)
        {
            Title = "ユーザー登録画面";
            SystemRoles = GetSystemRoles();
        }

        public async Task<IActionResult> OnPost()
        {
            // TODO: 入力チェック
            // TODO: 完了したことがわかる挙動

            var insertUserData = new GraphUserJsonData
            {
                EmployeeId = Request.Form["UserData.EmployeeId"],
                LastName = Request.Form["UserData.LastName"],
                FirstName = Request.Form["UserData.FirstName"],

                // TODO: 将来的にユーザー登録はアプリのDBに入れる＋ADに登録、としたい。ADに登録するときにメールアドレスを使う
                MailAddress = Request.Form["UserData.MailAddress"]
            };

            var insertSystemRoleValue = Request.Form["SystemRoleValue"];

            var slackId = await GetSlackId(insertUserData.MailAddress);

            InsertUser(insertUserData, insertSystemRoleValue, slackId);

            return Page();
        }

        // TODO: DB操作系のクラス整理
        private List<SelectListItem> GetSystemRoles()
        {
            var ret = new List<SelectListItem>();

            try
            {
                //call helper & get data
                SystemRolesHelper dbHelper = new SystemRolesHelper(_appSettings.Secrets.DbConnectionString);
                var dataList = dbHelper.SelectData();

                //loop & set
                foreach (var data in dataList)
                {
                    ret.Add(new SelectListItem()
                    {
                        Value = data.Id,
                        Text = data.Name
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetSystemRoles: {0}", ex.Message);
            }

            return ret;
        }

        private int InsertUser(GraphUserJsonData userData, string systemRoleValue, string slackId)
        {
            var ret = -1;

            try
            {
                //call helper & insert data
                UsersHelper dbHelper = new UsersHelper(_appSettings.Secrets.DbConnectionString);
                ret = dbHelper.InsertData(userData.EmployeeId, userData.LastName, userData.FirstName, systemRoleValue, slackId);

                _logger.LogDebug("Result count: {0}", ret);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetSystemRoles: {0}", ex.Message);
            }

            return ret;
        }

        private async Task<string> GetSlackId(string inputMailAddress)
        {
            var ret = "";

            try
            {
                var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                query.Add("token", _appSettings.Secrets.SlackAppToken);

                var requestUri = new UriBuilder()
                {
                    Scheme = Uri.UriSchemeHttps,
                    Host = "slack.com",
                    Path = "/api/users.list",
                    Query = query.ToString(),
                }.Uri.AbsoluteUri;

                using var client = new HttpClient();
                using var slackRequest = new HttpRequestMessage(HttpMethod.Post, requestUri);

                HttpResponseMessage slackResponse = await client.SendAsync(slackRequest);

                var getSlackResult = await slackResponse.Content.ReadAsStringAsync();

                var deserializeObj = JsonSerializer.Deserialize<SlackJsonData>(getSlackResult);

                if (deserializeObj.Result)
                {
                    foreach (var member in deserializeObj.Members)
                    {
                        if (inputMailAddress.Equals(member.Profile.Email))
                        {
                            ret = member.Id;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetSlackId: {0}", ex.Message);
            }

            return ret;
        }
    }
}
