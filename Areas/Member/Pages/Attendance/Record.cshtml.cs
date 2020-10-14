using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

using KintaiSystem.Models;
using KintaiSystem.DB;
using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Constants = KintaiSystem.Infrastructure.Constants;

namespace KintaiSystem.Areas.Member.Pages.Attendance
{
    public class RecordModel : KintaiSystemPageModelBase
    {
        public string CurrentDate { get; set; }
        public string ViewDate { get; set; }
        public bool DisabledBtnStart { get; set; }
        public bool DisabledBtnEnd { get; set; }
        public DateTime dt = DateTime.Now;

        public RecordModel(IWebHostEnvironment env, IConfiguration configuration, ILogger<RecordModel> logger)
            : base(env, configuration, logger)
        {
            Title = "出退勤記録";
            CurrentDate = dt.ToString("yyyyMMdd");
            ViewDate = dt.ToString("yyyy年MM月dd日(ddd)");
        }

        public IActionResult OnGet()
        {
            try
            {
                ChangeBtnStatus();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnGetAsync: {0}", ex.Message);
            }

            return Page();
        }

        public IActionResult OnPostStart()
        {
            try
            {
                AttendanceRecordsHelper dbHelper = new AttendanceRecordsHelper(_appSettings.Secrets.DbConnectionString);
                dbHelper.InsertData(HttpContext.Session.GetString(Constants.Session.EmployeeId), CurrentDate);

                ChangeBtnStatus();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnPost: {0}", ex.Message);
            }

            return Page();
        }

        public IActionResult OnPostEnd()
        {
            try
            {
                AttendanceRecordsHelper dbHelper = new AttendanceRecordsHelper(_appSettings.Secrets.DbConnectionString);
                dbHelper.UpdateData(HttpContext.Session.GetString(Constants.Session.EmployeeId), CurrentDate);

                ChangeBtnStatus();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error OnPost: {0}", ex.Message);
            }

            return Page();
        }

        private void ChangeBtnStatus()
        {
            AttendanceRecordsHelper dbHelper = new AttendanceRecordsHelper(_appSettings.Secrets.DbConnectionString);
            AttendanceRecordsData data = dbHelper.SelectData(HttpContext.Session.GetString(Constants.Session.EmployeeId), CurrentDate);

            DisabledBtnStart = data.StartFlg;
            DisabledBtnEnd = data.EndFlg;
        }
    }
}
