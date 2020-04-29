using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace week_10_web_crawler
{
    public static class cs_Global_Variables
    {
        public static int irMax_Concurrent_Task_Count = 20;
        public static int irPerThreadStartMiliSeconds = 100;
        public static int irMaxRetryCount = 3;
        public static int irMaxWaitHours = 24;

        public static object _obj_DicCrawlingUrls_lock = new object();
        public static Dictionary<string, per_Crawl_URL> dicCrawlingURLs = new Dictionary<string, per_Crawl_URL>();
        public static List<Task> lstRunningTasks = new List<Task>();

        public static HashSet<string> hsCrawledUrls = new HashSet<string>();
        public static HashSet<string> hsNewUrls = new HashSet<string>();
        public static HashSet<string> hsCurrentlyCrawlingUrl = new HashSet<string>();

        public static bool blSaveHtmlSource = false;
    }
}
