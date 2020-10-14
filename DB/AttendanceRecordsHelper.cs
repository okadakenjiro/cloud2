using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class AttendanceRecordsHelper
    {
        private readonly DatabaseHelper dbHelper;

        public AttendanceRecordsHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public AttendanceRecordsData SelectData(string employeeId, string currentDate)
        {
            var data = new AttendanceRecordsData();

            using var dataTable = dbHelper.GetDataTable($"select * from AttendanceRecords where UserId = '{employeeId}' and WorkDate = '{currentDate}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.UserId = row["UserId"].ToString();
                data.WorkDate = row["WorkDate"].ToString();
                data.StartDate = row["StartDate"].ToString();
                data.EndDate = row["EndDate"].ToString();
                data.StartFlg = bool.Parse(row["StartFlg"].ToString());
                data.EndFlg = bool.Parse(row["EndFlg"].ToString());
            }

            return data;
        }

        public int InsertData(string employeeId, string currentDate)
        {
            var ret = -1;

            var query = $"insert into AttendanceRecords (UserId, WorkDate, StartDate, StartFlg) " +
                            $"values ('{employeeId}', '{currentDate}', '{DateTime.Now}', 'true')";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }

        public int UpdateData(string employeeId, string currentDate)
        {
            var ret = -1;

            var query =　"update AttendanceRecords set " +
                            $"EndDate = '{DateTime.Now}', EndFlg = 'true' where UserId = '{employeeId}' and WorkDate = '{currentDate}'";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }

        public List<AttendanceRecordsData> SelectFullData(string employeeId)
        {
            var dataList = new List<AttendanceRecordsData>();

            using var dataTable = dbHelper.GetDataTable($"select * from AttendanceRecords where UserId = '{employeeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                AttendanceRecordsData data = new AttendanceRecordsData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.UserId = row["UserId"].ToString();
                data.WorkDate = row["WorkDate"].ToString();
                data.StartDate = row["StartDate"].ToString();
                data.EndDate = row["EndDate"].ToString();
                data.StartFlg = bool.Parse(row["StartFlg"].ToString());
                data.EndFlg = bool.Parse(row["EndFlg"].ToString());
                dataList.Add(data);
            }

            return dataList;
        }
    }
}