using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using KintaiSystem.Infrastructure.Json;
using KintaiSystem.Models;
using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using KintaiSystem.Infrastructure;
using System.Data;
using KintaiSystem.DB;
using System.ComponentModel.DataAnnotations;

namespace KintaiSystem.Areas.Member.Pages.ProjectUptime
{
    [ValidateAntiForgeryToken]
    public class RegisterModel : KintaiSystemPageModelBase
    {
        [BindProperty]
        public string TargetDay { get; set; }
        [BindProperty]
        public List<UserProjectUptimes> ProjectList { get; set; }
        // 登録情報のリスト
        public class UserProjectUptimes
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public int? PrimaryId { get; set; }
            public DateTime? WorkDate { get; set; }
            public string UserId { get; set; }
            [Required(ErrorMessage = "{0}は、必ず入力してください。")]
            [RegularExpression(@"^\d{1,2}(\.\d)?$", ErrorMessage = "{0}は、小数第一位までの数値で入力してください。")]
            [Display(Name = "作業時間")]
            public decimal? WorkMinute { get; set; }
            public string Remarks { get; set; }
        }
        
        public RegisterModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<RegisterModel> logger)
            : base(env, configuration, logger)
        {
            Title = "プロジェクト稼働時間登録";
        }

        public void OnGet()
        {
            try
            {
                if (string.IsNullOrEmpty(Request.Query["TargetDay"]))
                {
                    // 初期表示
                    TargetDay = DateTime.Today.ToString("yyyy/MM/dd");
                }
                else
                {
                    TargetDay = DateTime.Parse(Request.Query["TargetDay"]).ToString("yyyy/MM/dd");
                }

                // プロジェクト検索
                ProjectList = GetProjectList(DateTime.Parse(TargetDay)).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnGetAsync: {0}", ex.Message);
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                if (!IsValid())
                {
                    // プロジェクト名を再取得
                    ProjectList = ProjectList.Join(GetProjectList(DateTime.Parse(TargetDay)), list => list.ProjectId, dbList => dbList.ProjectId, (list, dbList) =>
                    {
                        list.ProjectName = dbList.ProjectName;
                        return list;
                    }).ToList();

                    return Page();
                }
                else
                {
                    // プロジェクト稼働時間登録処理
                    RegisterProjectUptimes();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnPost: {0}", ex.Message);
            }

            // 再表示
            return RedirectToPage("/ProjectUptime/Register", new { TargetDay=TargetDay });
        }

        private bool IsValid()
        {
            if (!ModelState.IsValid) { return false; }

            return true;
        }

        private IEnumerable<UserProjectUptimes> GetProjectList(DateTime workDate)
        {
            List<UserProjectUptimes> userProjectList = new List<UserProjectUptimes>();

            try
            {
                var dbHelper = new DatabaseHelper(_appSettings.Secrets.DbConnectionString);

                var employeeId = HttpContext.Session.GetString(Constants.Session.EmployeeId);

                using var dataTable = dbHelper.GetDataTable($"select Projects.Id as ProjectId" +
                                                            $"	   , Projects.Name as ProjectName" +
                                                            $"     , ProjectUptimes.PrimaryId" +
                                                            $"     , ProjectUptimes.WorkDate" +
                                                            $"     , ProjectUptimes.UserId" +
                                                            $"     , ProjectUptimes.WorkMinute / CONVERT(float, 60) as WorkMinute" +
                                                            $"     , ProjectUptimes.Remarks" +
                                                            $"  from Projects" +
                                                            $" inner join ProjectMembers" +
                                                            $"    on Projects.Id = ProjectMembers.ProjectId" +
                                                            $"   and ProjectMembers.StartDate <= '{workDate}'" +
                                                            $"   and ProjectMembers.EndDate > '{workDate}'" +
                                                            $"  left join ProjectUptimes" +
                                                            $"    on ProjectMembers.ProjectId = ProjectUptimes.ProjectId" +
                                                            $"   and ProjectMembers.UserId = ProjectUptimes.UserId" +
                                                            $"   and ProjectUptimes.WorkDate = '{workDate}'" +
                                                            $" where ProjectMembers.UserId = '{employeeId}'");

                foreach (var row in dataTable.Rows.Cast<DataRow>())
                {
                    UserProjectUptimes userProject = new UserProjectUptimes();

                    userProject.ProjectId = row["ProjectId"].ToString();
                    userProject.ProjectName = row["ProjectName"].ToString();
                    userProject.PrimaryId = DBNull.Value.Equals(row["PrimaryId"]) ? (int?)null : int.Parse(row["PrimaryId"].ToString());
                    userProject.WorkDate = DBNull.Value.Equals(row["WorkDate"]) ? (DateTime?)null : DateTime.Parse(row["WorkDate"].ToString());
                    userProject.UserId = row["UserId"].ToString();
                    userProject.WorkMinute = DBNull.Value.Equals(row["WorkMinute"]) ? (decimal?)null : decimal.Parse(row["WorkMinute"].ToString());
                    userProject.Remarks = row["Remarks"].ToString();

                    userProjectList.Add(userProject);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error GetProjectList: {0}", ex.Message);
            }

            return userProjectList;
        }

        private void RegisterProjectUptimes()
        {
            ProjectUptimesHelper dbHelper = new ProjectUptimesHelper(_appSettings.Secrets.DbConnectionString);

            try
            {
                foreach (var item in ProjectList)
                {
                    ProjectUptimesData data = new ProjectUptimesData();
                    data.WorkDate = DateTime.Parse(TargetDay);
                    data.ProjectId = item.ProjectId;
                    data.UserId = HttpContext.Session.GetString(Constants.Session.EmployeeId);
                    data.WorkMinute = (int)(item.WorkMinute * 60);
                    data.Remarks = item.Remarks;

                    if (item.PrimaryId.Equals(null))
                    {
                        dbHelper.InsertData(data);
                    }
                    else
                    {
                        var key = (int)item.PrimaryId;
                        dbHelper.UpdateData(data, key);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error RegisterProjectUptimes: {0}", ex.Message);
            }

        }

    }
}
