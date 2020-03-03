using System;
using System.Collections.Generic;
using System.Data;
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
using static week_5.csPublicVars;

namespace week_5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            tabStudents.IsEnabled = false;
            tabTeachers.IsEnabled = false;
            tabAdmin.IsEnabled = false;

            initVariables();
        }

        private string srEmailDefaultText = "enter your email";

        private void initVariables()
        {
            lstUserRanks = new List<csUserRanks>
            {
                new csUserRanks { irRank = -1, srRankTitle = "Select Rank" }
            };

            DataTable dtResult = csDbConnection.inlineSelectDataSet("select RankId,RankDescription from tblUserRanks").Tables[0];

            foreach (DataRow drw in dtResult.Rows)
            {
                lstUserRanks.Add(new csUserRanks
                {
                    irRank =
                    Convert.ToInt32(drw["RankId"].ToString()),
                    srRankTitle = drw["RankDescription"].ToString()
                });
            }

            txtUserEmail.Text = srEmailDefaultText;
        }

        private void txtUserEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            txtUserEmail.Text = "";
        }

        private void txtUserEmail_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserEmail.Text))
                txtUserEmail.Text = srEmailDefaultText;
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            secureLogin();


        }

        private void insecureLogin()
        {
            var vrEnteredPassword = txtPassword.Password.ToString().Trim().sha256();
            //vrEnteredPassword = csExtensionMethods.sha256(vrEnteredPassword);

            //this inline queries are open to SQL injection
            //never use this for parameters taken from users
            //use inline queries only when you are not using parameters from users
            DataTable dtResult = csDbConnection.inlineSelectDataSet($"select name,surname,user_rank from tblUsers where user_email='{txtUserEmail.Text}' and user_password='{vrEnteredPassword}'").Tables[0];

            if (dtResult.Rows.Count == 0)
            {
                MessageBox.Show("username or password is incorrect");
                return;
            }
        }

  

        private void secureLogin()
        {
            var vrEnteredPassword = txtPassword.Password.ToString().Trim().sha256();

            //we are going write a parameterized sql query

            string commandText = "select name,surname,user_rank from tblUsers where user_email=@user_email and user_password=@user_password";

            DataTable dtResult = csDbConnection.Parameterized_Select(commandText,
                new List<Tuple<string, object>> {
                new Tuple<string, object>("@user_email", txtUserEmail.Text),
                new Tuple<string, object>("@user_password",vrEnteredPassword)
                });

            //using (SqlConnection connection = new SqlConnection(csDbConnection.srConnectionString))
            //{
            //    SqlCommand command = new SqlCommand(commandText, connection);
            //    command.Parameters.AddWithValue("@user_email", txtUserEmail.Text);
            //    command.Parameters.AddWithValue("@user_password", vrEnteredPassword);
            //    try
            //    {
            //        connection.Open();
            //        dtResult.Load(command.ExecuteReader());
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.Message);
            //    }
            //}

            if(dtResult.Rows.Count==0)
            {
                LoggedUserInfo = null;
                MessageBox.Show("invalid password or username");
                return;
            }

            DataRow drwUserInfo = dtResult.Rows[0];

            LoggedUserInfo = new csLoggedUser();
            LoggedUserInfo.srUserFirstName = drwUserInfo["name"].ToString();
            LoggedUserInfo.srUserSurname = drwUserInfo["surname"].ToString();
            LoggedUserInfo.UserRankInfo = new csUserRanks { irRank = Convert.ToInt32(drwUserInfo["user_rank"].ToString()), srRankTitle = returnUserRankTitle(drwUserInfo["user_rank"].ToString()) };

            updateLoginLabel();
        }

        private void updateLoginLabel()
        {
            if(LoggedUserInfo!=null)
            {
                lblInfo.Content = $"Logged User: {LoggedUserInfo.srUserFirstName} {LoggedUserInfo.srUserSurname} - Rank: {LoggedUserInfo.UserRankInfo.srRankTitle}";
                tabLogin.IsEnabled = false;
                switch (LoggedUserInfo.UserRankInfo.irRank)
                {
                    case 1:
                        tabAdmin.IsEnabled = true;
                        tabAdmin.IsSelected = true;
                        break;
                    case 2:
                        tabTeachers.IsEnabled = true;
                        tabTeachers.IsSelected = true;
                        break;
                    case 3:
                        tabStudents.IsEnabled = true;
                        tabStudents.IsSelected = true;
                        break;
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoggedUserInfo = null;
            tabStudents.IsEnabled = false;
            tabAdmin.IsEnabled = false;
            tabTeachers.IsEnabled = false;
            tabLogin.IsEnabled = true;
            tabLogin.IsSelected = true;
        }
    }
}
