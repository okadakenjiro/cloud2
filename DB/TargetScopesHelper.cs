using System;
using System.Data;
using System.Linq;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class TargetScopesHelper
    {
        private readonly DatabaseHelper dbHelper;

        public TargetScopesHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public TargetScopesData SelectData(string scopeName)
        {
            var data = new TargetScopesData();

            using var dataTable = dbHelper.GetDataTable($"select * from TargetScopes where Name = '{scopeName}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Name = row["Name"].ToString();
                data.Remarks = row["Remarks"].ToString();
            }

            return data;
        }
    }
}