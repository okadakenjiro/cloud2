using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class SystemRolesHelper
    {
        private readonly DatabaseHelper dbHelper;

        public SystemRolesHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public List<SystemRolesData> SelectData()
        {
            var dataList = new List<SystemRolesData>();

            using var dataTable = dbHelper.GetDataTable(@"select * from SystemRoles order by PrimaryId Asc");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                var data = new SystemRolesData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Id = row["Id"].ToString();
                data.Name = row["Name"].ToString();
                dataList.Add(data);
            }

            return dataList;
        }
    }
}