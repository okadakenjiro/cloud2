using System;
using System.Data;
using System.Linq;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class PermittedScopesHelper
    {
        private readonly DatabaseHelper dbHelper;

        public PermittedScopesHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public PermittedScopesData SelectData(string systemRoleId, string scopeName)
        {
            var data = new PermittedScopesData();

            using var dataTable = dbHelper.GetDataTable($"select * from PermittedScopes where SystemRoleId = '{systemRoleId}' and ScopeName = '{scopeName}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.SystemRoleId = row["SystemRoleId"].ToString();
                data.ScopeName = row["ScopeName"].ToString();
            }

            return data;
        }

        public bool IsExistsData(string systemRoleId, string scopeName)
        {
            using var dataTable = dbHelper.GetDataTable($"select PrimaryId from PermittedScopes where SystemRoleId = '{systemRoleId}' and ScopeName = '{scopeName}'");

            return dataTable.Rows.Count > 0;
        }
    }
}