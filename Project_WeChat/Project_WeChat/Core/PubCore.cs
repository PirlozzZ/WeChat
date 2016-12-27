using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public string PubAuth(string sTimeStampm, string sNonce, string sMsgEncrypt, string sMsgSignature)
        {
            string sEchoStr = string.Empty;
            try
            {  
                Tencent.WXBizMsgCrypt wxcpt = new WXBizMsgCrypt(Config.getToken(),Config.getEncodingAESKey(),Config.getAppID());
                int ret = 0;
                ret = wxcpt.VerifyURL(sMsgSignature, sTimeStampm, sNonce, sMsgEncrypt, ref sEchoStr);
                if (ret != 0)
                {
                    log.Info("PubCore PubAuth VerifyURL failed"+ret); 
                } 
                log.Debug("PubCore PubAuth:"+Config.getToken()+"-"+ Config.getEncodingAESKey() + "-" + Config.getAppID());
            }
            catch (Exception e)
            {
                log.Error("PubCore PubAuth:", e);
            }
            return sEchoStr;
        }
    }
}