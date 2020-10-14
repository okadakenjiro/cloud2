using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using KintaiSystem.DB.Data;
using KintaiSystem.Helpers;

namespace KintaiSystem.DB
{
    public class ProjectUptimesHelper
    {
        private readonly DatabaseHelper dbHelper;

        public ProjectUptimesHelper(string connectionString)
        {
            dbHelper = new DatabaseHelper(connectionString);
        }

        public ProjectUptimesData SelectData(string employeeId)
        {
            var data = new ProjectUptimesData();

            using var dataTable = dbHelper.GetDataTable($"select * from ProjectUptimes where UserId = '{employeeId}'");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                data.PrimaryId = int.Parse(row["PrimaryId"].ToString());
                data.WorkDate = DateTime.Parse(row["WorkDate"].ToString());
                data.ProjectId = row["ProjectId"].ToString();
                data.UserId = row["UserId"].ToString();
                data.WorkMinute = int.Parse(row["WorkMinute"].ToString());
                data.Remarks = row["Remarks"].ToString();
            }

            return data;
        }

        public int InsertData(ProjectUptimesData data)
        {
            var ret = -1;

            var query = $"insert into ProjectUptimes (WorkDate, ProjectId, UserId, WorkMinute, Remarks) " +
                            $"values ('{data.WorkDate}', '{data.ProjectId}', '{data.UserId}', '{data.WorkMinute}', '{data.Remarks}')";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }

        public int UpdateData(ProjectUptimesData data, int key)
        {
            var ret = -1;

            var query = $"update ProjectUptimes" +
                        $"   set WorkDate = '{data.WorkDate}'" +
                        $"     , ProjectId = '{data.ProjectId}'" +
                        $"     , UserId = '{data.UserId}'" +
                        $"     , WorkMinute = '{data.WorkMinute}'" +
                        $"     , Remarks = '{data.Remarks}'" +
                        $" where PrimaryId = '{key}'";

            ret = dbHelper.ExecuteQuery(query);

            return ret;
        }

        public List<ProjectUptimesData> SelectProjectData(string projectId)
        {
            var dataList = new List<ProjectUptimesData>();

            using var dataTable = dbHelper.GetDataTable($"select ProjectId, UserId, Sum(WorkMinute) as TotalMinute from ProjectUptimes where ProjectId = '{projectId}' group by ProjectId, UserId order by ProjectId, UserId;");

            foreach (var row in dataTable.Rows.Cast<DataRow>())
            {
                var data = new ProjectUptimesData();
                data.ProjectId = row["ProjectId"].ToString();
                data.UserId = row["UserId"].ToString();
                data.WorkMinute = int.Parse(row["TotalMinute"].ToString());

                dataList.Add(data);
            }

            return dataList;
        }

    }
}