using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Tencent;

namespace WeChat.CorpLib.Core
{
    public class CorpCore
    {
        public DateTime sDateTime { get; private set; }
        public string sAccessToken
        {
            get
            {
                DateTime temp = DateTime.Now;
                if (DateTime.Compare(sDateTime.AddMinutes(7000), temp) < 0)
                {
                    sDateTime = temp;
                    GetAccessToken();
                }
                return sAccessToken;
            }
            private set { sAccessToken = value; }
        }
        private Config config;
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");
        bool isDES = bool.Parse(ConfigurationManager.AppSettings["isDES"]);
        bool isCustomerMsg = bool.Parse(ConfigurationManager.AppSettings["isCustomerMsg"]);
        WXBizMsgCrypt wxcpt;

        public CorpCore() : this("")
        {

        }

        public CorpCore(string sign)
        {
            log.Info("CorpCore refresh accesstoken!");
            config = new Config(sign);
            sDateTime = DateTime.Now;
            if (isDES)
            {
                wxcpt = new WXBizMsgCrypt(config.Token, config.EncodingAESKey, config.AppID);
            }
        }


        public void GetAccessToken()
        {
            try
            {
                log.Info("Refresh GetAccessToken!");
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", config.AppID, config.Secret);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                log.Debug(string.Format("GetAccessToken result: {0} ", result + "--" + url));
                JObject o = (JObject)JsonConvert.DeserializeObject(result);
                sAccessToken = o["access_token"].ToString();
            }
            catch (Exception err)
            {
                log.Error("GetAccessToken error!", err);
            }
        }

        public string CorpAuth(string sTimeStamp, string sNonce, string sEchoStr, string sMsgSignature)
        {
            string sReplyEchoStr = "";

            try
            {
                int ret = 0;
                ret = wxcpt.VerifyURL(sMsgSignature, sTimeStamp, sNonce, sEchoStr, ref sReplyEchoStr);
                if (ret != 0)
                {
                    log.Info("Refresh GetAccessToken!");
                }
            }
            catch (Exception e)
            {              
                log.Error("CorpAuth error:", e); 
            }
            return sReplyEchoStr;
        }

    }
}
