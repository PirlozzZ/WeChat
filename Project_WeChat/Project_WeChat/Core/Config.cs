 using System;
using System.Collections.Generic;
using System.Configuration;

namespace Project_WeChat.Core
{
    public class Config
    {
        
        private static string sAppID; 
        private static string sSecret;
        private static string sToken;
        private static string sEncodingAESKey;
       

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

            }
            return sToken;
        }

        public static string getAppID()
        {
            if (string.IsNullOrEmpty(sAppID))
            {  

            }
            return sAppID;
        }

        public static string getSecret()
        {
            if (string.IsNullOrEmpty(sSecret))
            {
               
            }
            return sSecret;
        }

        public static string getEncodingAESKey()
        {
            if (string.IsNullOrEmpty(sEncodingAESKey))
            { 
                
            }
            return sEncodingAESKey;
        }


    }
}