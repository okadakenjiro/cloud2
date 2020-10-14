using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class RequestTypesHelper
    {
        private readonly DatabaseHelper dbHelper;

        public RequestTypesHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public List<RequestTypesData> SelectData()
        {
            var dataList = new List<RequestTypesData>();

            using var dataTable = dbHelper.GetDataTable(@"select * from RequestTypes order by PrimaryId Asc");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                var data = new RequestTypesData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Id = row["Id"].ToString();
                data.Name = row["Name"].ToString();
                dataList.Add(data);
            }

            return dataList;
        }

        public RequestTypesData SelectSingleData(string requestTypeId)
        {
            var data = new RequestTypesData();

            using var dataTable = dbHelper.GetDataTable($"select * from RequestTypes where Id = '{requestTypeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.Id = row["Id"].ToString();
                data.Name = row["Name"].ToString();
            }

            return data;
        }
    }
}