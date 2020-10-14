using System;
using System.Data;
using System.Data.SqlClient;

namespace KintaiSystem.Helpers
{
    public class DatabaseHelper
    {
        private readonly string ConnectionString;

        public DatabaseHelper(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public DataTable GetDataTable(string sql)
        {
            //SqlDataReader ret = null;
            var ret = new DataSet();

            //open
            using var con = new SqlConnection(ConnectionString);
            con.Open();

            //commmand
            using var command = con.CreateCommand();
            command.CommandText = sql;

            var adapter = new SqlDataAdapter(command);
            adapter.Fill(ret);

            return ret.Tables[0];
        }

        public int ExecuteQuery(string sql)
        {
            using var con = new SqlConnection(ConnectionString);
            con.Open();

            //commmand
            using var command = con.CreateCommand();
            command.CommandText = sql;

            //reader
            return command.ExecuteNonQuery();
        }
    }
}