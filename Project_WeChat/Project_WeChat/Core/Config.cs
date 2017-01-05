 using System;
using System.Collections.Generic;
using System.Configuration;

namespace Project_WeChat.Core
{
    public class Config
    {        
        public Config():this("")
        {
            
        }

        public Config(string sign)
        {
            AppID = ConfigurationManager.AppSettings[sign + "pubAppID"];
            Secret = ConfigurationManager.AppSettings[sign + "pubSecret"];
            Token = ConfigurationManager.AppSettings[sign + "pubToken"];
            EncodingAESKey = ConfigurationManager.AppSettings[sign + "pubEncodingAESKey"];
        }

        public string AppID { get; private set; }
        public string Secret { get; private set; }
        public string Token { get; private set; }
        public string EncodingAESKey { get; private set; }


    }
}