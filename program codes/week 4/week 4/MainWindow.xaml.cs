using System;
using System.Collections.Generic;
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
            myClassOleDb.ole_Db_Adaptor.UpdateCommand = myClassOleDb.ole_DB_Builder.GetUpdateCommand();
            myClassOleDb.ole_Db_Adaptor.Update(myClassOleDb.ds_Ole_DB_Data_Set);
        }
    }
}
