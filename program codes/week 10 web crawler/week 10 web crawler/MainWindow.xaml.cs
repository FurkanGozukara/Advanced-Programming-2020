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
using static week_10_web_crawler.cs_public_functions;
using static week_10_web_crawler.cs_Global_Variables;
using System.Diagnostics;

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
            cs_public_functions.refMainWindow = this;
            for (int i = 0; i < cs_public_functions.csStatistics.irHowManyStatistics; i++)
            {
                lstboxStatistics.Items.Add("a statistic will be here");
            }

            if (cs_Global_Variables.blSaveHtmlSource)
                chkBoxSaveSourceCode.IsChecked = true;
        }

        private void btnSingleUrlCrawl_Click(object sender, RoutedEventArgs e)
        {
            doSingleCrawl(txtUrl.Text);
        }

        private void doSingleCrawl(string urlAddress)
        {
            //System.Threading.Thread.Sleep(10 * 1000);

            var vrResult = page_fetcher.fetch_a_page(urlAddress);

            if (vrResult.fetchStatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show((int)vrResult.fetchStatusCode + "\n" + vrResult.fetchStatusCode.ToString() +
                    "\n" +
                    vrResult.fetchStatusDescription + "\n" +
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
            startTimer();
        }

        private void startTimer()
        {
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(update_timer_label);
            dispatcherTimer.Tick += new EventHandler(updateStatistics);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer.Start();
        }

        private void updateStatistics(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var vrStatistics = cs_public_functions.returnStatistics();

                var vrThisSessionNewLinksCount = "new links found this session: " + vrStatistics.irThisSessionNewLinksCount.ToString("N0");

                var vrCrawlingCompletedThisSessionCount = "crawling completed link count this session: " + vrStatistics.irCrawlingCompletedThisSessionCount.ToString("N0");

                var vrCurrentlyCrawlingUrlsCount = "currently crawling urls count: " + vrStatistics.irCurrentlyCrawlingUrlsCount.ToString("N0");

                var vrTotalCrawlingCount = "all time crawling completed urls count: " + vrStatistics.irTotalCrawledUrlsCount.ToString("N0");

                var vrTotalUnCrawledUrlsCount = "all time waiting to be crawling urls count: " + vrStatistics.irTotalUnCrawledUrlsCount.ToString("N0");

                var vrAllTimeUrlsCount = "all time urls count: " + vrStatistics.irAllTimeUrlsCount.ToString("N0");


                lstboxStatistics.Items[0] = vrThisSessionNewLinksCount;
                lstboxStatistics.Items[1] = vrCrawlingCompletedThisSessionCount;
                lstboxStatistics.Items[2] = vrCurrentlyCrawlingUrlsCount;
                lstboxStatistics.Items[3] = vrTotalCrawlingCount;
                lstboxStatistics.Items[4] = vrTotalUnCrawledUrlsCount;
                lstboxStatistics.Items[5] = vrAllTimeUrlsCount;

            }));
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
                MessageBox.Show("task has finished: " + task.Status + " " + DateTime.Now);
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

        DispatcherTimer dispatchherCrawling = new DispatcherTimer();

        private void btnStartMainCrawling_Click(object sender, RoutedEventArgs e)
        {
            startTimer();
            cs_public_functions.loadCrawlingDictionary();

            dispatchherCrawling.Tick += new EventHandler(doMainCrawling);
            dispatchherCrawling.Interval = new TimeSpan(0, 0, 0, 0, cs_Global_Variables.irPerThreadStartMiliSeconds);
            dispatchherCrawling.Start();
        }

        private void doMainCrawling(object sender, EventArgs e)
        {
            for (int i = 0; i < cs_Global_Variables.irMax_Concurrent_Task_Count; i++)
            {


                Debug.WriteLine($"doMainCrawling called " + DateTime.Now.ToLocalTime() + " " + DateTime.Now.Millisecond);

                if (!checkCrawlingCanBeStarted())
                {
                    return;
                }

                string srNewUrl = null;

                lock (hsNewUrls)
                {
                    if (hsNewUrls.Count == 0)
                    {
                        lock (hsCurrentlyCrawlingUrl)
                            if (hsCurrentlyCrawlingUrl.Count == 0)
                            {
                                dispatchherCrawling.Stop();
                                saveCrawlingDictionary();
                                MessageBox.Show("crawling completed");
                                return;
                            }
                    }

                    foreach (var vrNewUrl in hsNewUrls)
                    {
                        if (hsCurrentlyCrawlingUrl.Contains(vrNewUrl))
                            continue;
                        srNewUrl = vrNewUrl;
                        break;
                    }

                    if (string.IsNullOrEmpty(srNewUrl))
                        return;

                    hsNewUrls.Remove(srNewUrl);

                    Task.Factory.StartNew(() =>
                    {
                        cs_public_functions.crawlURL(srNewUrl);
                    });

                }
            }
        }

        private void btnStopCrawling_Click(object sender, RoutedEventArgs e)
        {
            blCrawlingStopped = true;
            lock (hsCrawledUrls)
            {
                hsNewUrls = new HashSet<string>();
            }
            cs_public_functions.updateListStatusBox("software stop command is set please wait");
        }

        private void chkBoxSaveSourceCode_Click(object sender, RoutedEventArgs e)
        {
            if (chkBoxSaveSourceCode.IsChecked == true)
                blSaveHtmlSource = true;
            else
                blSaveHtmlSource = false;
        }
    }
}
