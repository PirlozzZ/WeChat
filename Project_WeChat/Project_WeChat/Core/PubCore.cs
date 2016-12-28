using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Tencent;

namespace Project_WeChat.Core
{
    public class PubCore
    {
        private static string sAccessToken;
        private Config config;
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");

        public PubCore():this("")
        {

        }

        public PubCore(string sign)
        {
            config = new Config(sign);

        }

        private string getAccessToken()
        {
            if (string.IsNullOrEmpty(sAccessToken))
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", Config.getAppID(), Config.getSecret());
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject o = (JObject)JsonConvert.DeserializeObject(result);
                sAccessToken = o["access_token"].ToString();
                return sAccessToken;
            }
            return sAccessToken;
        }

        public bool PubAuth(string sTimeStampm, string sNonce, string sMsgEncrypt, string sMsgSignature)
        {
            bool sign=true;
            try
            {
                List<string> tempList = new List<string>();
                tempList.Add(Config.getToken());
                tempList.Add(sTimeStampm);
                tempList.Add(sNonce);
                tempList.Sort();
                string tempStr = string.Empty;
                foreach (string _s in tempList)
                tempStr += _s;
                tempStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tempStr, "SHA1").ToLower();
                if (!tempStr.Equals(sMsgSignature))
                {
                    sign = false; 
                }
                log.Debug("PubCore PubAuth2:"+Config.getToken()+"-"+ Config.getEncodingAESKey() + "-" + Config.getAppID());
            }
            catch (Exception e)
            {
                log.Error("PubCore PubAuth:", e);
            }
            return sign;
        }
    }
}