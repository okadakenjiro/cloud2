using System;
using System.Data;
using System.Linq;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class DepartmentsHelper
    {
        private readonly DatabaseHelper dbHelper;

        public DepartmentsHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public DepartmentsData SelectData(string departmentId)
        {
            var data = new DepartmentsData();

            using var dataTable = dbHelper.GetDataTable($"select * from Departments where Id = '{departmentId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Id = row["Id"].ToString();
                data.Name = row["Name"].ToString();
                data.LeadUserId = row["LeadUserId"].ToString();
            }

            return data;
        }
    }
}