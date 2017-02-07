using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project_WeChat.Menu;
using Project_WeChat.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Web.Security;
using System.Xml;
using Tencent;

namespace Project_WeChat.Core
{
    public class PubCore
    {
        private static string sAccessToken;
        private Config config;
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");
        bool isDES = bool.Parse(ConfigurationManager.AppSettings["isDES"]);
        WXBizMsgCrypt wxcpt;

        public PubCore() : this("")
        {

        }

        public PubCore(string sign)
        {
            log.Info("PubCore refresh accesstoken!");
            config = new Config(sign);
            int expires_in = Int32.Parse(ConfigurationManager.AppSettings["expires_in"]);
            System.Timers.Timer t = new System.Timers.Timer(expires_in);//实例化Timer类，设置间隔时间；
            t.Elapsed += new System.Timers.ElapsedEventHandler(AutoRefreshAccessToken);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
            t.Start();
            GetAccessToken();
            if (isDES)
            {
                wxcpt = new WXBizMsgCrypt(config.Token, config.EncodingAESKey, config.AppID);
            } 
        }

        private void AutoRefreshAccessToken(object source, System.Timers.ElapsedEventArgs e)
        {
            log.Debug(string.Format("AutoRefreshAccessToken before: {0} ", sAccessToken));
            GetAccessToken();
            log.Debug(string.Format("AutoRefreshAccessToken after: {0} ", sAccessToken));     
        }

        private void GetAccessToken()
        { 
            try
            {
                if (string.IsNullOrEmpty(sAccessToken))
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", config.AppID, config.Secret);
                    string result = string.Empty;
                    result = HTTPHelper.GetRequest(url);
                    log.Debug(string.Format("GetAccessToken result: {0} ", result + "--" + url));
                    JObject o = (JObject)JsonConvert.DeserializeObject(result);
                    sAccessToken = o["access_token"].ToString(); 
                }
            }
            catch (Exception err)
            {
                log.Error("GetAccessToken error!", err);
            }
        }

        public bool PubAuth(string sTimeStamp, string sNonce, string sMsgEncrypt, string sMsgSignature)
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
                log.Debug("PubCore PubAuth2:" + sTimeStamp + "-" + sNonce + "-" + sMsgEncrypt);
            }
            catch (Exception e)
            {
                log.Error("PubCore PubAuth:", e);
            }
            return sign;
        }

        public string DecryptMsg(string sMsgSignature, string sTimeStamp, string sNonce, string postStr)
        {
            string strReuslt = postStr;
            try
            {
                if (isDES)
                {
                    int ret = 0;
                    ret = wxcpt.DecryptMsg(sMsgSignature, sTimeStamp, sNonce, postStr, ref strReuslt);
                    log.Debug("DecryptMsg Msg:" + postStr);
                    if (ret != 0)
                    {
                        log.Info("PubCore DecryptMsg failed");  
                    }
                }
                return strReuslt;
            }
            catch (Exception e)
            {
                log.Error("PubCore DecryptMsg:", e);
                return strReuslt;
            }
        }

        public bool ProcessMsg(string postStr, string sMsgSignature, string pTimeStamp, string pNonce)
        {
            bool sign = true;
            string sMsgType = string.Empty;
            string sEventType = string.Empty;
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
                    type = assembly.GetType("Project_WeChat.Model.PubRecEvent" + sEventType.Substring(0, 1).ToUpper() + sEventType.Substring(1).ToLower()); 
                }
                else
                {
                    type = assembly.GetType("Project_WeChat.Model.PubRecMsg" + sMsgType.Substring(0, 1).ToUpper() + sMsgType.Substring(1).ToLower());
                }
                log.Debug("ReflectClassName:" + type.Name);
                object instance = Activator.CreateInstance(type, new object[] { postStr });
                if (instance != null)
                {
                    PubRecAbstract temp = (PubRecAbstract)instance;
                    temp.DoProcess();
                    log.Debug("ProcessMsg instance:" + instance.ToString());
                }
            }
            catch (Exception e)
            {
                sign = false;
                log.Error("PubCore ProcessMsg:", e);             
            }
            return sign;
        }

        #region 菜单管理
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="root">RootMenu：创建默认菜单；ConditionalRootMenu：创建个性化菜单</param>
        /// <returns></returns>
        public bool CreateMenu(RootMenu root)
        {
            bool sign = false;
            string result = string.Empty;
            string strJson = JsonConvert.SerializeObject(root);
            bool isDefault = root.GetType() == typeof(RootMenu);
            log.Debug("createMenu strjson:" + strJson);
            try
            {
                string strType = (isDefault ? "create" : "addconditional");
                log.Debug("createMenu type:" + strType + "---" + root.GetType());
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/{0}?access_token={1}", strType, sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, strJson);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (isDefault)
                {
                    if ("ok".Equals(jo["errmsg"].ToString()))
                    {
                        sign = true;
                    }
                    else
                    {
                        log.Info(string.Format("createMenu Failed: {0} ", result));
                    }
                }
                else
                {
                    if (jo.Count==1)
                    {
                        sign = true;
                    }
                    else
                    {
                        log.Info(string.Format("createConditionalMenu Failed: {0} ", result));
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("createMenu Error", e);
            }
            return sign;
        }
        
        #endregion
    }
}