using System;
using System.Data;
using System.Linq;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class DepartmentMembersHelper
    {
        private readonly DatabaseHelper dbHelper;

        public DepartmentMembersHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public DepartmentMembersData SelectData(string employeeId)
        {
            var data = new DepartmentMembersData();

            using var dataTable = dbHelper.GetDataTable($"select * from DepartmentMembers where UserId = '{employeeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.DepartmentId = row["DepartmentId"].ToString();
                data.UserId = row["UserId"].ToString();
            }

            return data;
        }
    }
}