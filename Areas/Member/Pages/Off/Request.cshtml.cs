using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

using KintaiSystem.Models;
using KintaiSystem.DB;
using KintaiSystem.DB.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Constants = KintaiSystem.Infrastructure.Constants;

namespace KintaiSystem.Areas.Member.Pages.Off
{
    public class RequestModel : KintaiSystemPageModelBase
    {
        public List<SelectListItem> RequestTypes { get; set; } = new List<SelectListItem>();
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Reason { get; set; }

        public RequestModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<RequestModel> logger)
            : base(env, configuration, logger)
        {
            Title = "休暇申請";
            FromDate = DateTime.Now.ToString("yyyy/MM/dd");
            ToDate = DateTime.Now.ToString("yyyy/MM/dd");
            RequestTypes = GetRequestTypes();
        }

        public IActionResult OnPost()
        {
            try
            {
                var departmentMembersData = GetDepartmentMembers();
                var departmentsData = GetDepartments(departmentMembersData.DepartmentId);

                sendOffRequest(departmentsData.LeadUserId);
                runLogicApps(departmentsData.LeadUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnPost: {0}", ex.Message);
            }

            return Page();
        }


        private void sendOffRequest(string leadUserId)
        {
            try
            {
                var ScheduledDays = CalcScheduledDays();

                for (int i = 0; i < ScheduledDays; i++)
                {
                    OffRequestsData data = new OffRequestsData();
                    data.UserId = HttpContext.Session.GetString(Constants.Session.EmployeeId);
                    data.RequestTypeId = Request.Form["RequestTypes"];
                    data.ScheduledDate = DateTime.Parse(Request.Form["FromDate"]).AddDays(i);
                    data.Reason = Request.Form["Reason"];
                    data.ApprovalId = leadUserId;

                    InsertOffRequests(data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sendOffRequest: {0}", ex.Message);
            }
        }

        private int CalcScheduledDays()
        {
            var ret = -1;

            try
            {
                var fromDate = DateTime.Parse(Request.Form["FromDate"]);
                var toDate = DateTime.Parse(Request.Form["ToDate"]);

                ret = (int)((toDate - fromDate).TotalDays + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error CalcScheduledDays: {0}", ex.Message);
            }

            return ret;
        }

        private async void runLogicApps(string leadUserId)
        {
            try
            {
                var query = System.Web.HttpUtility.ParseQueryString(string.Empty);
                query.Add("api-version", "2016-10-01");
                query.Add("sp", "/triggers/manual/run");
                query.Add("sv", "1.0");
                query.Add("sig", "iI-HS1udKp13iuQ0tO0qFvIxdT4T1MZCHWEFzgoCqQs");
                query.Add("user", leadUserId);

                var requestLogicAppUri = new UriBuilder()
                {
                    Scheme = Uri.UriSchemeHttps,
                    Host = "prod-00.japaneast.logic.azure.com",
                    Path = "/workflows/f43cd7d4eae047118a31a267c2be5de6/triggers/manual/paths/invoke",
                    Query = query.ToString(),
                }.Uri.AbsoluteUri;

                using var client = new HttpClient();
                using var logicAppRequest = new HttpRequestMessage(HttpMethod.Post, requestLogicAppUri);

                await client.SendAsync(logicAppRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error runLogicApps: {0}", ex.Message);
            }
        }

        private List<SelectListItem> GetRequestTypes()
        {
            var ret = new List<SelectListItem>();

            try
            {
                RequestTypesHelper dbHelper = new RequestTypesHelper(_appSettings.Secrets.DbConnectionString);
                var dataList = dbHelper.SelectData();

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
                _logger.LogError("Error GetRequestTypes: {0}", ex.Message);
            }

            return ret;
        }

        private int InsertOffRequests(OffRequestsData data)
        {
            var ret = -1;

            try
            {
                OffRequestsHelper dbHelper = new OffRequestsHelper(_appSettings.Secrets.DbConnectionString);
                ret = dbHelper.InsertData(data);

                _logger.LogDebug("Result count: {0}", ret);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error InsertOffRequests: {0}", ex.Message);
            }

            return ret;
        }

        private DepartmentsData GetDepartments(string departmentId)
        {
            var data = new DepartmentsData();

            try
            {
                DepartmentsHelper dbHelper = new DepartmentsHelper(_appSettings.Secrets.DbConnectionString);
                data = dbHelper.SelectData(departmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetDepartments: {0}", ex.Message);
            }

            return data;
        }

        private DepartmentMembersData GetDepartmentMembers()
        {
            var data = new DepartmentMembersData();

            try
            {
                DepartmentMembersHelper dbHelper = new DepartmentMembersHelper(_appSettings.Secrets.DbConnectionString);
                data = dbHelper.SelectData(HttpContext.Session.GetString(Constants.Session.EmployeeId));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetDepartmentMembers: {0}", ex.Message);
            }

            return data;
        }
    }
}
