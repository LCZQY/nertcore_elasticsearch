using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ElasticseatchApi.Common
{
    /// <summary>
    /// 网络请求
    /// </summary>
    public class HttclientRequest
    {

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static T GetResponse<T>(string url,string postDataStr = "")
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + (postDataStr == "" ? "" : "?") + postDataStr);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";
                request.Timeout = 10000;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                T result = default(T);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.UTF8);
                    string retString = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();
                    result = JsonOperator.JsonDeserialize<T>(retString);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }                   
        }

         /// <summary>
         /// POST请求
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="url"></param>
         /// <param name="postDataStr"></param>
         /// <returns></returns>
        public async static Task<T> PostResponse<T>(string url, string postDataStr)
        {
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Timeout = 6000000;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataStr.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.UTF8);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                T result = default(T);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                    string retString = reader.ReadToEnd();
                    result = JsonOperator.JsonDeserialize<T>(retString);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
