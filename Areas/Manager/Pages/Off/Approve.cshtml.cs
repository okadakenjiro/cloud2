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

namespace KintaiSystem.Areas.Manager.Pages.Off
{
    public class ApproveModel : KintaiSystemPageModelBase
    {
        public List<ApproveViewData> viewDataList { get; set; }
        public string targetEmployeeId { get; set; }
        public string targetScheduledDate { get; set; }

        public ApproveModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<ApproveModel> logger)
            : base(env, configuration, logger)
        {
            Title = "休暇承認";
            viewDataList = new List<ApproveViewData>();
        }

        public IActionResult OnGet()
        {
            try
            {
                var dataList = GetOffRequestList();
                editViewDataListList(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnGet: {0}", ex.Message);
            }

            return Page();
        }

        public IActionResult OnPostApprove()
        {
            try
            {
                targetEmployeeId = Request.Form["targetEmployeeId"];
                targetScheduledDate = Request.Form["targetScheduledDate"];
                UpdateOffRequests(targetEmployeeId, DateTime.Parse(targetScheduledDate), 1);

                var dataList = GetOffRequestList();
                editViewDataListList(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnPostApprove: {0}", ex.Message);
            }

            return Page();
        }

        public IActionResult OnPostReject()
        {
            try
            {
                targetEmployeeId = Request.Form["targetEmployeeId"];
                targetScheduledDate = Request.Form["targetScheduledDate"];
                UpdateOffRequests(targetEmployeeId, DateTime.Parse(targetScheduledDate), 2);

                var dataList = GetOffRequestList();
                editViewDataListList(dataList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnPostReject: {0}", ex.Message);
            }

            return Page();
        }

        private void editViewDataListList(List<OffRequestsData> dataList)
        {
            foreach (var data in dataList)
            {
                var viewData = new ApproveViewData();
                var usersData = GetUsers(data.UserId);
                var requestTypesData = GetRequestTypes(data.RequestTypeId);

                viewData.EmployeeId = usersData.EmployeeId;
                viewData.Name = usersData.LastName + "　" + usersData.FirstName;
                viewData.RequestTypeName = requestTypesData.Name;
                viewData.ScheduledDate = data.ScheduledDate.ToString("yyyy/MM/dd");
                viewData.Reason = data.Reason;
                viewData.ApprovalFlg = data.ApprovalFlg == 0 ? "未" : (data.ApprovalFlg == 1 ? "済" : "否");

                viewDataList.Add(viewData);
            }
        }

        private List<OffRequestsData> GetOffRequestList()
        {
            var dataList = new List<OffRequestsData>();

            try
            {
                OffRequestsHelper dbHelper = new OffRequestsHelper(_appSettings.Secrets.DbConnectionString);
                dataList = dbHelper.SelectApprovalData(HttpContext.Session.GetString(Constants.Session.EmployeeId));
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

        private void UpdateOffRequests(string employeeId, DateTime scheduledDate, int approvalFlg)
        {
            try
            {
                OffRequestsHelper dbHelper = new OffRequestsHelper(_appSettings.Secrets.DbConnectionString);
                dbHelper.UpdateData(employeeId, scheduledDate, approvalFlg);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error UpdateOffRequests: {0}", ex.Message);
            }
        }

        public class ApproveViewData
        {
            public string EmployeeId { get; set; }
            public string Name { get; set; }
            public string RequestTypeName { get; set; }
            public string ScheduledDate { get; set; }
            public string Reason { get; set; }
            public string ApprovalFlg { get; set; }
        }
    }
}
