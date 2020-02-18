using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;


    public static class csDbConnection
    {
        //write full server path if your connection not working
        //run this query to get server full path : select @@SERVERNAME
        private static string srConnectionString =
      "server=localhost;database=okul;Integrated Security=SSPI;Connection Timeout=3000;";

    //to find out data source execute select @@servername
    //data source is host name and initial catalog is database name
    private static string srOledDBCon = @"Provider=sqloledb;Data Source=DESKTOP-ULH4M26;Initial Catalog=okul;Integrated Security=SSPI;";

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
                File.AppendAllText("connection errors.txt", $"{E.Message.ToString()} \t {E.StackTrace.ToString()}\r\n");
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

        public static Tuple<DataSet, OleDbDataAdapter, OleDbCommandBuilder> returnOleDB_Object(string srQuery)
        {
            DataSet data_set_local = new DataSet();
            OleDbDataAdapter ole_db_adapter = new OleDbDataAdapter();
            OleDbCommandBuilder ole_db_command_mybuilder = new OleDbCommandBuilder();
            OleDbConnection connection =  new OleDbConnection(srOledDBCon);
            connection.Open();

            ole_db_adapter.SelectCommand = new OleDbCommand(srQuery, connection);
            ole_db_command_mybuilder = new OleDbCommandBuilder(ole_db_adapter);
            ole_db_adapter.Fill(data_set_local);

            return new Tuple<DataSet, OleDbDataAdapter, OleDbCommandBuilder>(data_set_local, ole_db_adapter, ole_db_command_mybuilder);
        }

    public static csOleDbVariables returnOleDB_Class(string srQuery)
    {
        DataSet data_set_local = new DataSet();
        OleDbDataAdapter ole_db_adapter = new OleDbDataAdapter();
        OleDbCommandBuilder ole_db_command_mybuilder = new OleDbCommandBuilder();
        OleDbConnection connection = new OleDbConnection(srOledDBCon);
        connection.Open();

        ole_db_adapter.SelectCommand = new OleDbCommand(srQuery, connection);
        ole_db_command_mybuilder = new OleDbCommandBuilder(ole_db_adapter);
        ole_db_adapter.Fill(data_set_local);

        return new csOleDbVariables { ds_Ole_DB_Data_Set= data_set_local, ole_Db_Adaptor= ole_db_adapter,  ole_DB_Builder= ole_db_command_mybuilder };
    }
}

