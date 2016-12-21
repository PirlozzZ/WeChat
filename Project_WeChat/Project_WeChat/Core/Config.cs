using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Project_WeChat.Core
{
    public class Config
    {
        
        private static string sAppID; 
        private static string sSecret;
        private static string sToken;
        private static string sEncodingAESKey;
        private static string sAccessToken;

        public Config():this("")
        {
            
        }

        public Config(string sign)
        {
            sAppID = ConfigurationManager.AppSettings[sign + "pubAppID"];
            sSecret = ConfigurationManager.AppSettings[sign + "pubSecret"];
            sToken = ConfigurationManager.AppSettings[sign + "pubToken"];
            sEncodingAESKey = ConfigurationManager.AppSettings[sign + "pubEncodingAESKey"];
        }

        public static string getToken()
        {
            if (string.IsNullOrEmpty(sToken))
            { 
                if (string.IsNullOrEmpty(sToken))
                {
                     
                }
            }
            return sToken;
        }

        public static string getsCorpID()
        {
            if (string.IsNullOrEmpty(sAppID))
            { 
                if (string.IsNullOrEmpty(sAppID))
                {
                     
                }
            }
            return sAppID;
        }

        public static string getEncodingAESKey()
        {
            if (string.IsNullOrEmpty(sEncodingAESKey))
            { 
                if (string.IsNullOrEmpty(sEncodingAESKey))
                {
                     
                }
            }
            return sEncodingAESKey;
        }

        public static string getAccessToken()
        {
            if (string.IsNullOrEmpty(sAccessToken))
            { 
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", sAppID, sSecret);
                WebRequest webRequest = WebRequest.Create(url);
                webRequest.ContentType = "application/json";
                webRequest.Method = "Get";
                string temp = string.Empty;
                using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    temp = reader.ReadToEnd();
                }
                JObject o = (JObject)JsonConvert.DeserializeObject(temp);
                sAccessToken = o["access_token"].ToString(); 
                return sAccessToken;
            }
            return sAccessToken;
        }
    }
}