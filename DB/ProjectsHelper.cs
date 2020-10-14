using System.Data;
using System.Linq;
using System.Collections.Generic;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class ProjectsHelper
    {
        private readonly DatabaseHelper dbHelper;

        public ProjectsHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public List<ProjectsData> SelectData(string employeeId)
        {
            var dataList = new List<ProjectsData>();

            using var dataTable = dbHelper.GetDataTable($"select * from Projects where LeadUserId = '{employeeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                var data = new ProjectsData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Id = row["Id"].ToString();
                data.Name = row["Name"].ToString();
                data.LeadUserId = row["LeadUserId"].ToString();
                data.ScheduledCost = row["ScheduledCost"].ToString();
                dataList.Add(data);
            }

            return dataList;
        }

        public List<ProjectsData> SelectFullData()
        {
            var dataList = new List<ProjectsData>();

            using var dataTable = dbHelper.GetDataTable($"select * from Projects");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                var data = new ProjectsData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Id = row["Id"].ToString();
                data.Name = row["Name"].ToString();
                data.LeadUserId = row["LeadUserId"].ToString();
                data.ScheduledCost = row["ScheduledCost"].ToString();
                dataList.Add(data);
            }

            return dataList;
        }
    }
}