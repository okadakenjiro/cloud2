using System;
using System.Data;
using System.Linq;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class UsersHelper
    {
        private readonly DatabaseHelper dbHelper;

        public UsersHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public UsersData SelectData(string employeeId)
        {
            var data = new UsersData();

            using var dataTable = dbHelper.GetDataTable($"select * from Users where EmployeeId = '{employeeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.EmployeeId = row["EmployeeId"].ToString();
                data.LastName = row["LastName"].ToString();
                data.FirstName = row["FirstName"].ToString();
                data.SystemRoleId = row["SystemRoleId"].ToString();
                data.SlackId = row["SlackId"].ToString();
            }

            return data;
        }

        public int InsertData(string employeeId, string lastName, string firstName, string systemRole, string slackid)
        {
            var ret = -1;

            var query = $"insert into Users (EmployeeId, LastName, FirstName, SystemRoleId, SlackId) " +
                            $"values ('{employeeId}', '{lastName}', '{firstName}', '{systemRole}', '{slackid}')";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }
    }
}