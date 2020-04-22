﻿using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Threading;
using static week_10_web_crawler.cs_Global_Variables;

namespace week_10_web_crawler
{
    public static class cs_public_functions
    {
        public static MainWindow refMainWindow;
        private static readonly string srDicFilePath = "crawling_dictionary.txt";
        private static StreamWriter swLogs = new StreamWriter("logs.txt", true);
        private static StreamWriter swNewFoundUrls = new StreamWriter("new_urls_logs.txt", true);

        private static object _lock_swLogs = new object();
        private static object _lock_swNewFoundUrls = new object();

        static cs_public_functions()
        {
            swLogs.AutoFlush = true;
            swNewFoundUrls.AutoFlush = true;
        }

        private enum enLogType
        {
            ByPassedUrl,
            FoundUrl
        }

        private static void writeToLogsFile(string srLog, enLogType whichLog)
        {
            switch (whichLog)
            {
                case enLogType.ByPassedUrl:
                    lock (_lock_swLogs)
                    {
                        swLogs.WriteLine(srLog + "\t" + DateTime.Now + "\r\n");
                    }
                    break;
                case enLogType.FoundUrl:
                    lock (_lock_swNewFoundUrls)
                    {
                        swNewFoundUrls.WriteLine(srLog + "\t" + DateTime.Now + "\r\n");
                    }
                    break;
                default:
                    break;
            }
        }

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
                        addToDictionary(vrLine);
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
                    else
                        hsCrawledUrls.Add(vrPerUrl.Value.srURL);
                }
            }
        }

        private static void addToDictionary(string srUrl)
        {
            lock (_obj_DicCrawlingUrls_lock)
            {
                per_Crawl_URL myUrl = new per_Crawl_URL();
                myUrl.srNormalizedURL = srUrl.NormalizeURL();
                myUrl.srURL = srUrl;
                myUrl.srKey = srUrl.HashURL();
                myUrl.srRootSite = srUrl.GetRootURL();
                if (!dicCrawlingURLs.ContainsKey(myUrl.srKey))
                {
                    dicCrawlingURLs.Add(myUrl.srKey, myUrl);
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

        public static bool blCrawlingStopped = false;

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

            addToDictionary(srUrl);
            int irRetryCount = 0;
            lock (hsCurrentlyCrawlingUrl)
            {
                hsCurrentlyCrawlingUrl.Add(srUrl);
            }
            var vrUrlKey = srUrl.HashURL();
            lock (_obj_DicCrawlingUrls_lock)
            {
                var vrCurrent = dicCrawlingURLs[vrUrlKey];
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
                    var vrCurrent = dicCrawlingURLs[vrUrlKey];
                    vrCurrent.blCurrentlyCrawling = false;
                    vrCurrent.dtCrawlingEnded = DateTime.Now;
                    vrCurrent.srCrawledSource = vrFetchResult.srFetchSource;
                    vrCurrent.blCrawled = true;
                }

                lock (hsCrawledUrls)
                    hsCrawledUrls.Add(srUrl);

                var vrNewUrls = returnNewUrls(vrFetchResult.srFetchSource, srUrl);

                addNewUrlsToQueue(vrNewUrls);
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

        private static void addNewUrlsToQueue(List<string> lstNewUrls)
        {
            lock (hsCrawledUrls)
            {
                foreach (var vrUrl in lstNewUrls.ToList())
                {
                    if (hsCrawledUrls.Contains(vrUrl))
                        lstNewUrls.Remove(vrUrl);
                }

                foreach (var vrNewUrl in lstNewUrls)
                {
                    addToDictionary(vrNewUrl);
                }
              
                if (blCrawlingStopped == false)
                    lock (hsNewUrls)
                    {
                        foreach (var vrNewUrl in lstNewUrls)
                        {
                            hsNewUrls.Add(vrNewUrl);
                        }
                    }
            }
        }

        public static List<string> returnNewUrls(string srSource, string srCrawledUrl)
        {
            List<string> lstFoundNewUrls = new List<string>();
            HtmlDocument hdDoc = new HtmlDocument();
            hdDoc.LoadHtml(srSource);

            var vrNodes = hdDoc.DocumentNode.SelectNodes("//a[@href]");

            if (vrNodes != null)
                foreach (HtmlNode link in vrNodes)
                {
                    var vrHrefVal = link.Attributes["href"].Value.ToString();

                    var baseUrl = new Uri(srCrawledUrl);

                    Uri newUrl;
                    if (Uri.TryCreate(baseUrl, vrHrefVal, out newUrl))
                    {
                        string srNewUrl = newUrl.AbsoluteUri.ToString().urlNormalize();
                        if (checkIfUrlToBeCrawled(srCrawledUrl, srNewUrl, false))
                        {
                            writeToLogsFile(srNewUrl, enLogType.FoundUrl);
                            lstFoundNewUrls.Add(srNewUrl);
                        }
                    }
                }

            return lstFoundNewUrls;
        }

        private static string urlNormalize(this string srUrl)
        {
            return HttpUtility.HtmlDecode(HttpUtility.UrlDecode(srUrl)).Split('#').FirstOrDefault();
        }

        private static readonly List<string> lst_not_allowed_uri_extensions = new List<string>
        {
          ".png",".jpg",".jpeg",".css",".js",".pdf",".docx",".doc"
        };

        private static bool checkIfUrlToBeCrawled(string srCrawledUrl, string srNewUrl,
            bool blAllowExternalUrls = true)
        {
            Uri orgUrl = new Uri(srCrawledUrl);
            Uri newUrl = new Uri(srNewUrl);

            if (newUrl.Scheme != Uri.UriSchemeHttp && newUrl.Scheme != Uri.UriSchemeHttps)
            {
                writeToLogsFile($"Scheme {newUrl.Scheme.ToString()} not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                return false;
            }

            if (blAllowExternalUrls == false)
            {
                if (orgUrl.Host.ToString() != newUrl.Host.ToString())
                {
                    writeToLogsFile($"external links not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                    return false;
                }
            }

            foreach (var vrExtension in lst_not_allowed_uri_extensions)
            {
                if (srNewUrl.ToLowerInvariant().EndsWith(vrExtension))
                {
                    writeToLogsFile($"extension {vrExtension} not allowed url : {srNewUrl}", enLogType.ByPassedUrl);
                    return false;
                }
            }

            return true;
        }
    }
}
