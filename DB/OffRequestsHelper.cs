using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class OffRequestsHelper
    {
        private readonly DatabaseHelper dbHelper;

        public OffRequestsHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public List<OffRequestsData> SelectData(string employeeId)
        {
            var dataList = new List<OffRequestsData>();

            using var dataTable = dbHelper.GetDataTable($"select * from OffRequests where UserId = '{employeeId}' order by UserId, ScheduledDate DESC");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                OffRequestsData data = new OffRequestsData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.UserId = row["UserId"].ToString();
                data.RequestTypeId = row["RequestTypeId"].ToString();
                data.ScheduledDate = DateTime.Parse(row["ScheduledDate"].ToString());
                data.Reason = row["Reason"].ToString();
                data.ApprovalId = row["ApprovalId"].ToString();
                data.ApprovalFlg = int.Parse(row["ApprovalFlg"].ToString());
                if (!String.IsNullOrEmpty(row["ApprovalDate"].ToString()))
                {
                    data.ApprovalDate = DateTime.Parse(row["ApprovalDate"].ToString());
                }
                dataList.Add(data);
            }

            return dataList;
        }

        public int InsertData(OffRequestsData data)
        {
            var ret = -1;

            var query = $"insert into OffRequests (UserId, RequestTypeId, ScheduledDate, Reason, ApprovalId) " +
                            $"values ('{data.UserId}', '{data.RequestTypeId}', '{data.ScheduledDate}', '{data.Reason}', '{data.ApprovalId}')";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }

        public List<OffRequestsData> SelectApprovalData(string employeeId)
        {
            var dataList = new List<OffRequestsData>();

            using var dataTable = dbHelper.GetDataTable($"select * from OffRequests where ApprovalId = '{employeeId}' order by UserId, ScheduledDate DESC");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                OffRequestsData data = new OffRequestsData();
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.UserId = row["UserId"].ToString();
                data.RequestTypeId = row["RequestTypeId"].ToString();
                data.ScheduledDate = DateTime.Parse(row["ScheduledDate"].ToString());
                data.Reason = row["Reason"].ToString();
                data.ApprovalId = row["ApprovalId"].ToString();
                data.ApprovalFlg = int.Parse(row["ApprovalFlg"].ToString());
                if (!String.IsNullOrEmpty(row["ApprovalDate"].ToString()))
                {
                    data.ApprovalDate = DateTime.Parse(row["ApprovalDate"].ToString());
                }
                dataList.Add(data);
            }

            return dataList;
        }

        public int UpdateData(string employeeId, DateTime scheduledDate, int approvalFlg)
        {
            var ret = -1;

            var query = "update OffRequests set " +
                            $"ApprovalFlg = '{approvalFlg}', ApprovalDate = '{DateTime.Now}' where UserId = '{employeeId}' and ScheduledDate = '{scheduledDate}'";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }
    }
}