using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
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
            int expires_in = Int32.Parse(ConfigurationManager.AppSettings["expires_in"]);
            System.Timers.Timer t = new System.Timers.Timer(expires_in);//实例化Timer类，设置间隔时间为7000毫秒；
            t.Elapsed += new System.Timers.ElapsedEventHandler(getAccessToken);//到达时间的时候执行事件；
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；
        }

        private void getAccessToken(object source, System.Timers.ElapsedEventArgs e)
        {
            if (string.IsNullOrEmpty(sAccessToken))
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", Config.getAppID(), Config.getSecret());
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject o = (JObject)JsonConvert.DeserializeObject(result);
                sAccessToken = o["access_token"].ToString();             
            }
        }

        public bool PubAuth(string sTimeStamp, string sNonce, string sMsgEncrypt, string sMsgSignature)
        {
            bool sign=true;
            try
            { 
                List<string> tempList = new List<string>();
                tempList.Add(Config.getToken());
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
                log.Debug("PubCore PubAuth2:"+ sTimeStamp + "-"+ sNonce + "-" + sMsgEncrypt);
            }
            catch (Exception e)
            {
                log.Error("PubCore PubAuth:", e);
            }
            return sign;
        }

        #region 菜单管理
        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        public bool createMenu(RootMenu root)
        {
            bool sign = false;
            string result = string.Empty;
            string strJson = JsonConvert.SerializeObject(root);
            log.Debug("createMenu strjson:" + strJson);
            try
            { 
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, strJson);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("createMenu Failed: {0} ", result));
                }
            }
            catch (Exception e)
            {
                log.Error("createMenu Error",e);
            }
            return sign;
        }
        #endregion
    }
}