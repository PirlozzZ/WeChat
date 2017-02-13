using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WeChat.PubLib.Core
{
    public class HTTPHelper
    {
        /// <summary>
        /// Post带参请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="ContentType">指定参数类型</param>
        /// <param name="strData"></param>
        /// <returns></returns>
        public static string PostRequest(string url, DataTypeEnum ContentType, string strData)
        {
            string result = string.Empty;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method = MethodTypeEnum.Post.ToString();
            webRequest.ContentType = "application/" + ContentType.ToString();
            byte[] reqBodyBytes = System.Text.Encoding.UTF8.GetBytes(strData); //指定编码，微信用的是UTF8，我起初用的是default，以为默认是utf8的，后来发现这受操作系统影响的。
            Stream reqStream = webRequest.GetRequestStream();//加入需要发送的参数
            reqStream.Write(reqBodyBytes, 0, reqBodyBytes.Length);
            reqStream.Close();
            using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }


        /// <summary>
        /// Get不带参请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="method"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static string GetRequest(string url)
        {
            string result = string.Empty;
            WebRequest webRequest = WebRequest.Create(url);
            webRequest.Method =MethodTypeEnum.Get.ToString(); 
            using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }

    /// <summary>
    /// 带参数据类型
    /// </summary>
    public enum DataTypeEnum
    {
        json,
        xml
    }

    /// <summary>
    /// 带参数据类型
    /// </summary>
    public enum MethodTypeEnum
    {
        Get,
        Post
    }
}