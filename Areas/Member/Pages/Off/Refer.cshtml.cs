using System;
using System.Collections.Generic;

using KintaiSystem.Models;
using KintaiSystem.DB;
using KintaiSystem.DB.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Constants = KintaiSystem.Infrastructure.Constants;

namespace KintaiSystem.Areas.Member.Pages.Off
{
    public class ReferModel : KintaiSystemPageModelBase
    {
        public List<ReferViewData> viewDataList { get; set; }

        public ReferModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<ReferModel> logger)
            : base(env, configuration, logger)
        {
            Title = "申請一覧";
            viewDataList = new List<ReferViewData>();
        }

        public IActionResult OnGet()
        {
            try
            {
                var dataList = GetOffRequestList();
                EditViewDataListList(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnGet: {0}", ex.Message);
            }

            return Page();
        }

        private void EditViewDataListList(List<OffRequestsData> dataList)
        {
            foreach (var data in dataList)
            {
                var viewData = new ReferViewData();
                var usersData = GetUsers(data.ApprovalId);
                var requestTypesData = GetRequestTypes(data.RequestTypeId);

                viewData.RequestTypeName = requestTypesData.Name;
                viewData.ScheduledDate = data.ScheduledDate.ToString("yyyy/MM/dd");
                viewData.Reason = data.Reason;
                viewData.ApprovalName = usersData.LastName + "　" + usersData.FirstName;
                viewData.ApprovalMsg = data.ApprovalFlg == 0 ? "未確認です。" : (data.ApprovalFlg == 1 ? "承認されました。" : "拒否されました。");

                viewDataList.Add(viewData);
            }
        }

        private List<OffRequestsData> GetOffRequestList()
        {
            var dataList = new List<OffRequestsData>();

            try
            {
                OffRequestsHelper dbHelper = new OffRequestsHelper(_appSettings.Secrets.DbConnectionString);
                dataList = dbHelper.SelectData(HttpContext.Session.GetString(Constants.Session.EmployeeId));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetOffRequestList: {0}", ex.Message);
            }

            return dataList;
        }

        private UsersData GetUsers(string employeeId)
        {
            var data = new UsersData();

            try
            {
                UsersHelper dbHelper = new UsersHelper(_appSettings.Secrets.DbConnectionString);
                data = dbHelper.SelectData(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetUsers: {0}", ex.Message);
            }

            return data;
        }

        private RequestTypesData GetRequestTypes(string requestTypeId)
        {
            var data = new RequestTypesData();

            try
            {
                RequestTypesHelper dbHelper = new RequestTypesHelper(_appSettings.Secrets.DbConnectionString);
                data = dbHelper.SelectSingleData(requestTypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetRequestTypes: {0}", ex.Message);
            }

            return data;
        }

        public class ReferViewData
        {
            public string RequestTypeName { get; set; }
            public string ScheduledDate { get; set; }
            public string Reason { get; set; }
            public string ApprovalName { get; set; }
            public string ApprovalMsg { get; set; }
        }
    }
}
