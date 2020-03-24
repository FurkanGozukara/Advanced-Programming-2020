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
using System.Data;

namespace week_7
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

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            string srQuery = @"select [ProductID]
      ,[Name]
      ,[ProductNumber]
	  from [Production].[Product]
	  where [Name] like @name";

            var vrSearchParam = $"%{txtSearchBox.Text}%";

            DataTable dtResults = csDbConnection.Parameterized_Select(srQuery,
                new List<Tuple<string, object>>
                {
                   new Tuple<string, object>("@name",vrSearchParam)
                });

            dtgrid1.ItemsSource = dtResults.DefaultView;
        }

        //home work, make it show when multiple rows selected
        private void dtgrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string srSelectedId = "-1";
            try
            {
                var vr1 = dtgrid1.SelectedItem;
                var vr2 = dtgrid1.SelectedItems;

                System.Data.DataRowView vRow = (System.Data.DataRowView)vr1;
                srSelectedId = vRow.Row.ItemArray.FirstOrDefault().ToString();
            }
            catch
            {
                return;
            }


            string srQuery = @"select *
	  from [Production].[Product]
	 where ProductID=@productid";

            DataTable dtResults = csDbConnection.Parameterized_Select(srQuery,
                new List<Tuple<string, object>>
                {
                   new Tuple<string, object>("@productid",srSelectedId)
                });

            dtGrid2.ItemsSource = dtResults.DefaultView;
        }



        private void btnOpenSorting_Click(object sender, RoutedEventArgs e)
        {
            SortingExample win2 = new SortingExample();
            win2.Show();
        }
    }
}
