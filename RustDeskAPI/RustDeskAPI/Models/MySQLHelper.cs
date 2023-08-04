using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace RustDeskAPI.Models
{
    public class MySQLHelper
    {
        static string connstr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        public static object Query(string sql)
        {
            object result = null;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connstr))
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        result = cmd.ExecuteScalar();
                        return result;
                    }
                }
            }
            catch { return result; }
        }

        public static DataTable QueryDataTable(string sql)
        {
            DataTable result = new DataTable();
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connstr))
                {
                    conn.Open();
                    using (MySqlDataAdapter da = new MySqlDataAdapter(sql, conn))
                    {
                        da.Fill(result);
                        return result;
                    }
                }
            }
            catch { return result; }
        }

        public static int Execute(string sql)
        {
            int result = 0;
            try
            {
                using (MySqlConnection conn = new MySqlConnection(connstr))
                {
                    if (conn.State !=ConnectionState.Closed)
                    {
                        conn.Close();
                    }
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception ex)
            { return result; }
        }

    }

}