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
using System.Windows.Shapes;
using System.Data;
using System.ComponentModel;

namespace week_7
{
    /// <summary>
    /// Interaction logic for SortingExample.xaml
    /// </summary>
    public partial class SortingExample : Window
    {
        public SortingExample()
        {
            InitializeComponent();
            initComboBoxes();
        }

        Dictionary<string, string> dicComboBoxValues = new Dictionary<string, string>();

        private void initComboBoxes()
        {
            dicComboBoxValues.Add("NoSorting", "Select Sorting Type");
            
            foreach (enSorting vrSorting in (enSorting[])Enum.GetValues(typeof(enSorting)))
            {
                foreach (enSortDirection vrDirection in (enSortDirection[])Enum.GetValues(typeof(enSortDirection)))
                {
                    dicComboBoxValues.Add(vrSorting+"\t"+ vrDirection, vrSorting.GetDescription() + "\t\t" + vrDirection.GetDescription());
                }
            }

            cBoxFirstSorting.ItemsSource = dicComboBoxValues;
            cBoxFirstSorting.DisplayMemberPath = "Value";
            cBoxFirstSorting.SelectedValuePath = "Key";
            cBoxFirstSorting.SelectedIndex = 0;

            DataSet dsPersonTypes = csDbConnection.inlineSelectDataSet("select distinct PersonType from Person.Person order by PersonType asc");

            cBoxPersonTypeFiltering.Items.Add("Filter by Person Type");
            cBoxPersonTypeFiltering.SelectedIndex = 0;
            if (dsPersonTypes!=null)
                if(dsPersonTypes.Tables.Count>0)
            foreach (DataRow drw  in dsPersonTypes.Tables[0].Rows)
            {
               cBoxPersonTypeFiltering.Items.Add(drw["PersonType"].ToString());
            }
        }

        private void refreshPersonDataGrid()
        {
            string srQuery= "select top 100 * from [Person].[Person]{0}";

            if(cBoxFirstSorting.SelectedIndex>0)
            {
                srQuery = srQuery + " order by " + cBoxFirstSorting.SelectedValue.ToString().Split('\t')[0] + " " + cBoxFirstSorting.SelectedValue.ToString().Split('\t')[1];
            }

            if (cBoxPersonTypeFiltering.SelectedIndex > 0)
            {
                srQuery = string.Format(srQuery, $" where PersonType='{cBoxPersonTypeFiltering.SelectedItem.ToString()}' ");
            }
            else
                srQuery = string.Format(srQuery, ""); 

          DataSet dsPersons = csDbConnection.inlineSelectDataSet(srQuery);

            dtGridPersons.ItemsSource = dsPersons.Tables[0].DefaultView;
        }

        private void refreshPersonDataGrid_Secure()
        {
            List<Tuple<string, object>> lstParameters = new List<Tuple<string, object>>(); 

            string srQuery = "select top 100 * from [Person].[Person]{0}";

            if (cBoxFirstSorting.SelectedIndex > 0)
            {
                srQuery = srQuery + " order by " + cBoxFirstSorting.SelectedValue.ToString().Split('\t')[0] + " " + cBoxFirstSorting.SelectedValue.ToString().Split('\t')[1];
            }

            if (cBoxPersonTypeFiltering.SelectedIndex > 0)
            {
                srQuery = string.Format(srQuery, $" where PersonType=@PersonType ");
                lstParameters.Add(new Tuple<string, object> ( "@PersonType", cBoxPersonTypeFiltering.SelectedItem.ToString() ));
            }
            else
                srQuery = string.Format(srQuery, "");

            DataTable dtPersons = csDbConnection.Parameterized_Select(srQuery, lstParameters);

            dtGridPersons.ItemsSource = dtPersons.DefaultView;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            refreshPersonDataGrid();
      
        }

        enum enSorting
        {
            [Description("Person First Name")]
            FirstName,
            [Description("Person Middle Name")]
            MiddleName,
            [Description("Person Last Name")]
            LastName
        }

        enum enSortDirection
        {
            [Description("Ascending Order")]
            Asc,
            [Description("Descending Order")]
            Desc
        }

        private void safeRefresh_Click(object sender, RoutedEventArgs e)
        {
            refreshPersonDataGrid_Secure();
        }
    }
}
