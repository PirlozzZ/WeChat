using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using Tencent;
using System.Xml;
using System.Reflection;
using WeChat.CorpLib.Model;

namespace WeChat.CorpLib.Core
{
    public class CorpCore
    {
        public DateTime sDateTime { get; private set; }
        private string _sAccessToken;
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
                return _sAccessToken;
            }
            private set { _sAccessToken = value; }
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
                log.Info("CorpCore Refresh GetAccessToken!");
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", config.AppID, config.Secret);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                log.Debug(string.Format("CorpCore GetAccessToken result: {0} ", result + "--" + url));
                JObject o = (JObject)JsonConvert.DeserializeObject(result);
                sAccessToken = o["access_token"].ToString();
            }
            catch (Exception err)
            {
                log.Error("CorpCore GetAccessToken error!", err);
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
                    log.Info(string.Format("CorpAuth failed：{0} ",ret));
                }
            }
            catch (Exception e)
            {              
                log.Error("CorpAuth error:", e); 
            }
            return sReplyEchoStr;
        }

        /// <summary>
        /// 消息处理
        /// </summary>
        /// <param name="postStr"></param>
        /// <param name="sMsgSignature"></param>
        /// <param name="pTimeStamp"></param>
        /// <param name="pNonce"></param>
        /// <returns></returns>
        public string ProcessMsg(string postStr, string sMsgSignature, string pTimeStamp, string pNonce)
        {
            string sMsgType = string.Empty;
            string sEventType = string.Empty;
            string sResult = string.Empty;
            string sMsg = DecryptMsg(sMsgSignature, pTimeStamp, pNonce, postStr);  // 解析之后的明文
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sMsg);
                XmlNode root = doc.FirstChild;
                sMsgType = root["MsgType"].InnerText;
                Assembly assembly = Assembly.GetExecutingAssembly();
                Type type;
                if ("event".Equals(sMsgType))
                {
                    sEventType = root["Event"].InnerText;
                    type = assembly.GetType("WeChat.CorpLib.Model.PubRecEvent" + sEventType.Substring(0, 1).ToUpper() + sEventType.Substring(1).ToLower());
                }
                else
                {
                    type = assembly.GetType("WeChat.CorpLib.Model.PubRecMsg" + sMsgType.Substring(0, 1).ToUpper() + sMsgType.Substring(1).ToLower());
                }
                log.Debug("CorpCore ReflectClassName:" + type.Name);
                object instance = Activator.CreateInstance(type, new object[] { sMsg });
                if (instance != null)
                {
                    CorpRecAbstract temp = (CorpRecAbstract)instance;
                    sResult = temp.DoProcess();
                    if (string.IsNullOrEmpty(sResult))
                    {
                        sResult = "success";
                    }
                    log.Debug("CorpCore ProcessMsg instance:" + instance.ToString());
                }
            }
            catch (Exception e)
            {
                log.Error("CorpCore ProcessMsg:", e);
            }

            return EncryptMsg(pTimeStamp, pNonce, sResult);
        }

        #region 信息加密解密
        /// <summary>
        /// 解密信息
        /// </summary>
        /// <param name="sMsgSignature"></param>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="postStr"></param>
        /// <returns></returns>
        private string DecryptMsg(string sMsgSignature, string sTimeStamp, string sNonce, string postStr)
        {
            string strReuslt = postStr;
            try
            {
                if (isDES)
                {
                    int ret = 0;
                    ret = wxcpt.DecryptMsg(sMsgSignature, sTimeStamp, sNonce, postStr, ref strReuslt);
                    log.Debug("CorpCore DecryptMsg Msg:" + postStr);
                    if (ret != 0)
                    {
                        log.Info("CorpCore DecryptMsg failed");
                    }
                }
                return strReuslt;
            }
            catch (Exception e)
            {
                log.Error("CorpCore DecryptMsg:", e);
                return strReuslt;
            }
        }

        /// <summary>
        /// 加密信息
        /// </summary> 
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="postStr"></param>
        /// <returns></returns>
        private string EncryptMsg(string sTimeStamp, string sNonce, string postStr)
        {
            string strReuslt = postStr;
            try
            {
                if (isDES && (!"success".Equals(postStr)))
                {
                    int ret = 0;
                    ret = wxcpt.EncryptMsg(postStr, sTimeStamp, sNonce, ref strReuslt);
                    log.Debug("CorpCore EncryptMsg Msg:" + postStr);
                    if (ret != 0)
                    {
                        log.Info("CorpCore EncryptMsg failed");
                    }
                }
                return strReuslt;
            }
            catch (Exception e)
            {
                log.Error("CorpCore EncryptMsg:", e);
                return strReuslt;
            }
        }
        #endregion
    }
}
