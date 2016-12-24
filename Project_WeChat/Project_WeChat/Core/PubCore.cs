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

        public PubCore():this("")
        {

        }

        public PubCore(string sign)
        {
            config = new Config(sign);
        }

        private static string getAccessToken()
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

        public static bool PubAuth(string sTimeStampm, string sNonce, string sMsgEncrypt, string sMsgSignature)
        {
            bool result = false;
            string tempSignature = string.Empty;
            WXBizMsgCrypt.GenarateSinature(Config.getToken(), sTimeStampm, sNonce, sMsgEncrypt, ref tempSignature);
            if (tempSignature.Equals(sMsgSignature))
            {
                result = true;
            }
            return result;
        }
    }
}