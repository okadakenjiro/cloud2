using System;
using System.Data;
using System.Collections.Generic;

using KintaiSystem.Models;
using KintaiSystem.DB;
using KintaiSystem.DB.Data;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Constants = KintaiSystem.Infrastructure.Constants;

namespace KintaiSystem.Areas.Manager.Pages.Cost
{
    public class ReferModel : KintaiSystemPageModelBase
    {

        public List<RefereViewProjectData> viewDataList { get; set; }

        public ReferModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<ReferModel> logger)
            : base(env, configuration, logger)
        {
            Title = "工数管理";
            viewDataList = new List<RefereViewProjectData>();
        }

        public IActionResult OnGet()
        {
            try
            {
                var projectList = GetProjectList();
                EditViewDataList(projectList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnGet: {0}", ex.Message);
            }

            return Page();
        }

        private void EditViewDataList(List<ProjectsData> dataList)
        {
            foreach (var data in dataList)
            {
                var viewData = new RefereViewProjectData();
                var memberDataList = new List<RefereViewMemberData>();

                viewData.Name = data.Name;
                viewData.Id = data.Id;
                viewData.ScheduledCost = data.ScheduledCost;

                var totalCost = 0;
                var projectUptimesList = GetProjectUptimesList(data.Id);
                foreach (var uptimesData in projectUptimesList)
                {
                    totalCost += uptimesData.WorkMinute;

                    var memberData = new RefereViewMemberData();
                    memberData.Id = uptimesData.UserId;
                    memberData.WorkTime = (uptimesData.WorkMinute / 60).ToString();
                    memberDataList.Add(memberData);
                }
                viewData.TotalCost = (totalCost / 60).ToString();
                viewData.MemberList = memberDataList;

                viewDataList.Add(viewData);
            }
        }

        private List<ProjectsData> GetProjectList()
        {
            List<ProjectsData> dataList = new List<ProjectsData>();

            try
            {
                ProjectsHelper dbHelper = new ProjectsHelper(_appSettings.Secrets.DbConnectionString);

                if("M0000002".Equals(HttpContext.Session.GetString(Constants.Session.SystemRoleId)))
                {
                    dataList = dbHelper.SelectData(HttpContext.Session.GetString(Constants.Session.EmployeeId));
                } else
                {
                    dataList = dbHelper.SelectFullData();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetProjectList: {0}", ex.Message);
            }

            return dataList;
        }

        private List<ProjectUptimesData> GetProjectUptimesList(string projectId)
        {
            List<ProjectUptimesData> dataList = new List<ProjectUptimesData>();

            try
            {
                ProjectUptimesHelper dbHelper = new ProjectUptimesHelper(_appSettings.Secrets.DbConnectionString);
                dataList = dbHelper.SelectProjectData(projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetProjectUptimes: {0}", ex.Message);
            }

            return dataList;
        }

        public class RefereViewProjectData
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string ScheduledCost { get; set; }
            public string TotalCost { get; set; }
            public List<RefereViewMemberData> MemberList { get; set; }
        }

        public class RefereViewMemberData
        {
            public string Id { get; set; }
            public string WorkTime { get; set; }
        }
    }
}
