using System;
using System.Data;
using System.Linq;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class ProjectMembersHelper
    {
        private readonly DatabaseHelper dbHelper;

        public ProjectMembersHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public ProjectMembersData SelectData(string employeeId)
        {
            var data = new ProjectMembersData();

            using var dataTable = dbHelper.GetDataTable($"select * from ProjectMembers where UserId = '{employeeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.ProjectId = row["ProjectId"].ToString();
                data.UserId = row["UserId"].ToString();
            }

            return data;
        }
    }
}