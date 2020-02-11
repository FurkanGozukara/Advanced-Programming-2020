using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using static week_3_database_connect.csDbConnection;

namespace week_3_database_connect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            initComboBox();
        }

        private void initComboBox()
        {
            List<csUserRanks> lstUsers = new List<csUserRanks>
            {
                new csUserRanks { irRank = -1, srRankTitle = "Select Rank" },
                new csUserRanks { irRank = 1, srRankTitle = "Admin" },
                new csUserRanks { irRank = 2, srRankTitle = "Teacher" },
                new csUserRanks { irRank = 3, srRankTitle = "Student" }
            };

            cmbBoxUserRank.ItemsSource = lstUsers;
            cmbBoxUserRank.DisplayMemberPath = "srRankTitle";
            cmbBoxUserRank.SelectedValuePath = "irRank";
            cmbBoxUserRank.SelectedIndex = 0;
        }

        public class csUserRanks
        {
            public int irRank { get; set; }
            public string srRankTitle { get; set; }
        }

  

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            int irSelectedRank = (int) cmbBoxUserRank.SelectedValue;

            string srUserHashedPassword = sha256(pwUserPassword.Password.ToString());

            string srQuery = $@"  insert into tblUsers (user_email,user_password,name,surname,user_rank)
                values (N'{txtEmail.Text.Replace("'","''")}',
                N'{srUserHashedPassword}',
                    N'{txtFirstName.Text.Replace("'", "''")}',
                N'{txtSurname.Text.Replace("'", "''")}',
                {irSelectedRank})";

            inlineUpdateDeleteInsert(srQuery);

            //you have to either use using statement or explicity close sql connection
    
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
