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
    }
}
