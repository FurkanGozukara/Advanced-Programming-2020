using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace week_10_web_crawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ThreadPool.SetMaxThreads(100000, 100000);
            ThreadPool.SetMinThreads(100000, 100000);
        }

        private void btnSingleUrlCrawl_Click(object sender, RoutedEventArgs e)
        {
            doSingleCrawl(txtUrl.Text);
        }

        private void doSingleCrawl(string urlAddress)
        {
            //  System.Threading.Thread.Sleep(10 * 1000);

            var vrResult = page_fetcher.fetch_a_page(urlAddress);

            if (vrResult.fetchStatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show((int)vrResult.fetchStatusCode + "\n" + vrResult.fetchStatusCode.ToString() +
                    "\n" +
                    vrResult.fetchStatusDescription+"\n"+
                    vrResult.exceptionE.Message);
                return;
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                txtSourceCode.Text = vrResult.srFetchSource;
            }));
        }

        private void btnTimerStart_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(update_timer_label);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Start();
        }

        private void update_timer_label(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblTimer.Content = $"time: {DateTime.Now.ToString()} : {DateTime.Now.Millisecond}";
            }));
        }

        private void btnTaskBasedCrawl_Click(object sender, RoutedEventArgs e)
        {
            string srUrl = txtUrl.Text;
            Task vrStartedTask = Task.Factory.StartNew(() => { doSingleCrawl(srUrl); }).ContinueWith(task =>
            {
                MessageBox.Show("task has finished: " + task.Status+" "+DateTime.Now);
            });
            // vrStartedTask.Wait();
            MessageBox.Show("task started but not finished yet: " + vrStartedTask.Status + " " + DateTime.Now);
        }

        private void threadCountTest_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                Task.Factory.StartNew(() => { tempTask(); }, TaskCreationOptions.LongRunning);
            }
        }

        private void threadCountTestDefault_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 1000; i++)
            {
                Task.Factory.StartNew(() => { tempTask(); });
            }
        }

        private void tempTask()
        {
            System.Threading.Thread.Sleep(100 * 1000);
        }
    }
}
