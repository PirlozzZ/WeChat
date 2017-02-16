﻿ using System;
using System.Collections.Generic;
using System.Configuration;

namespace WeChat.CorpLib.Core
{
    public class Config
    {        
        public Config():this("")
        {
            
        }

        public Config(string sign)
        {
            AppID = ConfigurationManager.AppSettings[sign + "corpAppID"];
            Secret = ConfigurationManager.AppSettings[sign + "corpAppSecret"];
            Token = ConfigurationManager.AppSettings[sign + "corpToken"];
            EncodingAESKey = ConfigurationManager.AppSettings[sign + "corpEncodingAESKey"];
        }

        public string AppID { get; private set; }
        public string Secret { get; private set; }
        public string Token { get; private set; }
        public string EncodingAESKey { get; private set; }


    }
}