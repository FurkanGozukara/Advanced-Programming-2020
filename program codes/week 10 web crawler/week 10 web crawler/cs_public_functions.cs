using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using static week_10_web_crawler.cs_Global_Variables;

namespace week_10_web_crawler
{
    public static class cs_public_functions
    {
        public static MainWindow refMainWindow;
        private static readonly string srDicFilePath = "crawling_dictionary.txt";

        public static void loadCrawlingDictionary()
        {
            lock (_obj_DicCrawlingUrls_lock)
            {
                if (File.Exists(srDicFilePath))
                {
                    dicCrawlingURLs = JsonConvert.DeserializeObject<Dictionary<string, per_Crawl_URL>>(File.ReadAllText(srDicFilePath));
                }
                if (File.Exists("root_urls.txt"))
                {
                    foreach (var vrLine in File.ReadLines("root_urls.txt"))
                    {
                        per_Crawl_URL myUrl = new per_Crawl_URL();
                        myUrl.srNormalizedURL = vrLine.NormalizeURL();
                        myUrl.srURL = vrLine;
                        myUrl.srKey = vrLine.HashURL();
                        myUrl.srRootSite = vrLine.GetRootURL();
                        if (!dicCrawlingURLs.ContainsKey(myUrl.srKey))
                        {
                            dicCrawlingURLs.Add(myUrl.srKey, myUrl);
                        }
                    }
                }

                foreach (var vrPerUrl in dicCrawlingURLs)
                {
                    if (vrPerUrl.Value.blCrawled == false)
                    {
                        if (vrPerUrl.Value.irCrawlRetryCount >= irMaxRetryCount)
                        {
                            if (vrPerUrl.Value.dtCrawlingStarted.AddHours(irMaxWaitHours) > DateTime.Now)
                                continue;
                        }
                        hsNewUrls.Add(vrPerUrl.Value.srURL);
                    }
                }
            }
        }

        public static void saveCrawlingDictionary()
        {
            lock (_obj_DicCrawlingUrls_lock)
            {
                string json = JsonConvert.SerializeObject(dicCrawlingURLs, Formatting.Indented);
                File.WriteAllText(srDicFilePath, json);
            }
        }

        public static void updateListStatusBox(string srMsg)
        {
            refMainWindow.Dispatcher.BeginInvoke(new Action(() =>
            {
                refMainWindow.lstStatusBox.Items.Insert(0, $"{srMsg}\t{DateTime.Now}");
            }));
        }

        public static bool checkCrawlingCanBeStarted()
        {
            lock (lstRunningTasks)
            {
                //to list is necessary otherwise if you make changes to the list it will give error
                foreach (var vrTask in lstRunningTasks.ToList())
                {
                    if (vrTask.Status == TaskStatus.RanToCompletion ||
                       vrTask.Status == TaskStatus.Faulted ||
                      vrTask.Status == TaskStatus.Canceled)
                    {
                        lstRunningTasks.Remove(vrTask);
                    }
                }

                if (irMax_Concurrent_Task_Count > lstRunningTasks.Count)
                    return true;
                return false;
            }
        }

        public static void crawlURL(string srUrl)
        {
            updateListStatusBox("starting to crawl\t" + srUrl);

            per_Crawl_URL myUrl = new per_Crawl_URL();
            myUrl.srNormalizedURL = srUrl.NormalizeURL();
            myUrl.srURL = srUrl;
            myUrl.srKey = srUrl.HashURL();
            myUrl.srRootSite = srUrl.GetRootURL();
            int irRetryCount = 0;
            lock (hsCurrentlyCrawlingUrl)
            {
                hsCurrentlyCrawlingUrl.Add(srUrl);
            }
            lock (_obj_DicCrawlingUrls_lock)
            {
                if (!dicCrawlingURLs.ContainsKey(myUrl.srKey))
                {
                    dicCrawlingURLs.Add(myUrl.srKey, myUrl);
                }
                var vrCurrent = dicCrawlingURLs[myUrl.srKey];
                vrCurrent.blCurrentlyCrawling = true;
                vrCurrent.dtCrawlingStarted = DateTime.Now;
                vrCurrent.irCrawlRetryCount++;
                irRetryCount = vrCurrent.irCrawlRetryCount;
            }

            var vrFetchResult = page_fetcher.fetch_a_page(srUrl);

            if (vrFetchResult.fetchStatusCode == System.Net.HttpStatusCode.OK)
            {
                lock (_obj_DicCrawlingUrls_lock)
                {
                    var vrCurrent = dicCrawlingURLs[myUrl.srKey];
                    vrCurrent.blCurrentlyCrawling = false;
                    vrCurrent.dtCrawlingEnded = DateTime.Now;
                    vrCurrent.srCrawledSource = vrFetchResult.srFetchSource;
                    vrCurrent.blCrawled = true;
                }

                lock (hsCrawledUrls)
                    hsCrawledUrls.Add(srUrl);
            }
            else
            {
                if (irRetryCount < irMaxRetryCount)
                {
                    lock (hsNewUrls)
                        hsNewUrls.Add(srUrl);
                }
            }

            lock (hsCurrentlyCrawlingUrl)
                hsCurrentlyCrawlingUrl.Remove(srUrl);
        }
    }
}
