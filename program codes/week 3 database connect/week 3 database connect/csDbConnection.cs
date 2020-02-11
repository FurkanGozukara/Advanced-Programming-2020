using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace week_3_database_connect
{
    public static class csDbConnection
    {
        //write full server path if your connection not working
        //run this query to get server full path : select @@SERVERNAME
        private static string srConnectionString =
      "server=localhost;database=okul;Integrated Security=SSPI;Connection Timeout=3000;";

        public static int inlineUpdateDeleteInsert(string srQuery)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(srQuery, connection);
                    return command.ExecuteNonQuery();
                }
            }
            catch (Exception E)
            {
                File.AppendAllText("connection errors.txt",$"{E.Message.ToString()} \t {E.StackTrace.ToString()}\r\n");
                return -1;
            }    
        }

        public static DataSet inlineSelectDataSet(string srQuery)
        {
            //data set means more than 1 table
            //data table is only 1 table
            DataSet dSet = new DataSet();
            try
            {
                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    connection.Open();
                    SqlDataAdapter DA = new SqlDataAdapter(srQuery, srConnectionString);
                    DA.Fill(dSet);
                }
            }
            catch (Exception E)
            {
                File.AppendAllText("connection errors.txt", $"{E.Message.ToString()} \t {E.StackTrace.ToString()}\r\n");             
            }

            return dSet;
        }
    }
}
