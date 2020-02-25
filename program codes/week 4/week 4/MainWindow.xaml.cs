using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace week_4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Tuple<System.Data.DataSet, System.Data.OleDb.OleDbDataAdapter, System.Data.OleDb.OleDbCommandBuilder> tObjects;

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            tObjects = csDbConnection.returnOleDB_Object("select * from okul.dbo.tblUsers");
            dtGridUsers1.ItemsSource = tObjects.Item1.Tables[0].DefaultView;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            tObjects.Item2.UpdateCommand = tObjects.Item3.GetUpdateCommand();
            tObjects.Item2.Update(tObjects.Item1);
        }

        csOleDbVariables myClassOleDb;

        private void btnRefreshClass_Click(object sender, RoutedEventArgs e)
        {
            myClassOleDb = csDbConnection.returnOleDB_Class("select * from okul.dbo.tblUsers");
            dtGridUsers1.ItemsSource = myClassOleDb.ds_Ole_DB_Data_Set.Tables[0].DefaultView;
        }

        private void btnSaveWithclass_Click(object sender, RoutedEventArgs e)
        {
            foreach (DataTable dt in myClassOleDb.ds_Ole_DB_Data_Set.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    for (int irColumnNo = 0; irColumnNo < dt.Columns.Count; irColumnNo++)
                    {
                        var vrCurrent = row[irColumnNo, DataRowVersion.Current];
                        var vrOrg = row[irColumnNo, DataRowVersion.Original];
                        if (!vrCurrent.Equals(vrOrg))
                        {
                            if(dt.Columns[irColumnNo].ToString()=="user_password")
                            {
                                var vrPassword = row["user_password"].ToString().Trim();
                                var vrHashedPassword = sha256(vrPassword);
                                row["user_password"] = vrHashedPassword;
                            }
                        }
                    }
                }
            }

            myClassOleDb.ole_Db_Adaptor.UpdateCommand = myClassOleDb.ole_DB_Builder.GetUpdateCommand();
            myClassOleDb.ole_Db_Adaptor.Update(myClassOleDb.ds_Ole_DB_Data_Set);
        }

        static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}
