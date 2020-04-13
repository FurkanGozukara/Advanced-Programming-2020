using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace week_10_web_crawler
{
    public static class csExtensions
    {
        public static string NormalizeURL(this string srURL)
        {
            return srURL.Split('#').FirstOrDefault().ToLower(new System.Globalization.CultureInfo("en-us"));
        }
        public static string HashURL(this string srURL)
        {
            return srURL.NormalizeURL().ComputeSha256Hash();
        }
        public static string ComputeSha256Hash(this string rawString)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawString));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        public static string GetRootURL(this string srURL)
        {
            Uri myUri = new Uri(srURL);
            return myUri.Host;  // host is "www.contoso.com"
        }
    }
}
