using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace week_3_database_connect
{
    public static class csDbConnection
    {
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
            catch 
            {
                return -1;
            }    
        }
    }
}
