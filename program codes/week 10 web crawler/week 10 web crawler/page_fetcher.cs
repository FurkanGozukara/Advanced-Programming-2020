using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace week_10_web_crawler
{
    public class cs_fetch_result
    {
        public string srFetchSource { get; set; }
        public HttpStatusCode fetchStatusCode { get; set; }
        public string fetchStatusDescription { get; set; }
        public Exception exceptionE = null;
    }

    public static class page_fetcher
    {
        public static cs_fetch_result fetch_a_page(string urlAddress)
        {
            cs_fetch_result temp_cs_fetch_result = new cs_fetch_result();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream receiveStream = response.GetResponseStream())
                    {
                        StreamReader readStream = null;

                        if (String.IsNullOrWhiteSpace(response.CharacterSet))
                            readStream = new StreamReader(receiveStream);
                        else
                            readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

                        temp_cs_fetch_result.srFetchSource = readStream.ReadToEnd();

                        temp_cs_fetch_result.fetchStatusCode = response.StatusCode;
                    }
                }
            }
            catch (WebException e)
            {
                temp_cs_fetch_result.exceptionE = e;
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    temp_cs_fetch_result.fetchStatusCode = ((HttpWebResponse)e.Response).StatusCode;
                    temp_cs_fetch_result.fetchStatusDescription = ((HttpWebResponse)e.Response).StatusDescription;
                }
            }
            catch (Exception e)
            {
                temp_cs_fetch_result.exceptionE = e;
            }

            return temp_cs_fetch_result;
        }
    }
}
