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
                if (DateTime.Compare(sDateTime.AddMinutes(7000), DateTime.Now) < 0)
                {
                    sDateTime = DateTime.Now;
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

        public bool CorpAuth(string sTimeStamp, string sNonce, string sMsgEncrypt, string sMsgSignature)
        {
            bool sign = true;
            try
            {
                List<string> tempList = new List<string>();
                tempList.Add(config.Token);
                tempList.Add(sTimeStamp);
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
                log.Debug("CorpAuth:" + sTimeStamp + "-" + sNonce + "-" + sMsgEncrypt);
            }
            catch (Exception e)
            {
                log.Error("CorpAuth:", e);
            }
            return sign;
        }

    }
}
