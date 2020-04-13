using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace week_10_web_crawler
{
    public class per_Crawl_URL
    {
        public string srURL { get; set; }
        public string srNormalizedURL { get; set; }
        //srKey is sha256 hashed version of normalized url
        public string srKey { get; set; }
        public DateTime dtCrawlingStarted;
        public DateTime dtCrawlingEnded;
        public string srRootSite { get; set; }
        public string srCrawledSource { get; set; }
        public bool blCurrentlyCrawling = false;
        public int irCrawlRetryCount = 0;
        public bool blCrawled = false;
    }
}
