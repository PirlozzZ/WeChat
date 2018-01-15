using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WeChat.PubLib.Menu;
using WeChat.PubLib.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;
using Tencent;
using System.Configuration;
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace WeChat.PubLib.Core
{
    public class PubCore
    {
        private static DateTime sDateTime { get; set; }
        private string _sAccessToken;
        private string _sign = string.Empty;
        private ServerType _serverType = ServerType.LocalServer;
        public string sAccessToken
        {
            get { 
                try {
                    DateTime temp = DateTime.Now;
                    TimeSpan timespan = temp - sDateTime;
                    log.Debug("PubCore sAccessToken1 :" + sDateTime + "@@" + temp+"@@"+timespan.TotalMilliseconds+"@@"+config.expires_in);
                    if (timespan.TotalMilliseconds>= config.expires_in)
                    {
                        log.Debug("PubCore sAccessToken2 :" + sDateTime + "@@" + temp);              
                        GetAccessToken();
                        sDateTime = temp;
                        log.Debug("PubCore sAccessToken3 :" + sDateTime);
                    }
                }
                catch (Exception  e)
                {
                    log.Error("sAccessToken err:", e);
                }
                return _sAccessToken;
            }
            private set { _sAccessToken = value; }
        }
        private Config config;
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");
        bool isDES ;
        bool isCustomerMsg;
        WXBizMsgCrypt wxcpt;

        #region 构造方法
        public PubCore() : this("")
        {

        }

        public PubCore(string sign)
        {
            _sign = sign;
            config = new Config(sign);
            sDateTime = DateTime.Now;
            isDES = bool.Parse(ConfigurationManager.AppSettings[sign+"isDES"]);
            isCustomerMsg = bool.Parse(ConfigurationManager.AppSettings[sign+"isCustomerMsg"]);
            if (isDES)
            {
                wxcpt = new WXBizMsgCrypt(config.Token, config.EncodingAESKey, config.AppID);
            }
            if(sAccessToken==null)
            GetAccessToken();
        }

        public PubCore(string sign,ServerType serverType)
        {
            _sign = sign;
            _serverType = serverType;
            config = new Config(sign);
            sDateTime = DateTime.Now;
            isDES = bool.Parse(ConfigurationManager.AppSettings[sign + "isDES"]);
            isCustomerMsg = bool.Parse(ConfigurationManager.AppSettings[sign + "isCustomerMsg"]);
            if (isDES)
            {
                wxcpt = new WXBizMsgCrypt(config.Token, config.EncodingAESKey, config.AppID);
            }
            if (sAccessToken == null)
                GetAccessToken();
        }

        #endregion

        #region 核心方法
        /// <summary>
        /// 获取AccessToken 
        /// </summary>
        public void GetAccessToken()
        {
            try
            {
                log.Info(_sign+"|PubCore Refresh GetAccessToken!——sDateTime：" + sDateTime.ToString());
                if (ServerType.LocalServer == _serverType)
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", config.AppID, config.Secret);
                    string result = string.Empty;
                    result = HTTPHelper.GetRequest(url);
                    log.Debug(string.Format(_sign+"|PubCore GetAccessToken Local result: {0} ", result + "--" + url));
                    JObject o = (JObject)JsonConvert.DeserializeObject(result);
                    sAccessToken = o["access_token"].ToString();
                }
                else
                {
                    string url = string.Format("{0}?seaskyAccessToken={1}", ConfigurationManager.AppSettings["serverURL"], _sign);
                    sAccessToken = HTTPHelper.GetRequest(url);
                    log.Debug(string.Format(_sign+"|PubCore GetAccessToken Other result: {0} ", sAccessToken + "--" + url));
                }
            }
            catch (Exception err)
            {
                log.Error(_sign+"|PubCore GetAccessToken error!", err);
            }
        }


        /// <summary>
        /// 服务器验证
        /// </summary>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="sMsgEncrypt"></param>
        /// <param name="sMsgSignature"></param>
        /// <returns></returns>
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
                //.Net 4.5版本后下列方法过时
                //tempStr = FormsAuthentication.HashPasswordForStoringInConfigFile(tempStr, "SHA1").ToLower();

                #region key校验测试
                //DateTime now = DateTime.Now;
                //string personCode = string.Empty;
                //string key = "";
                //bool sign = false;
                //for(int i = -5; i <= 5; i++)
                //{
                //    DateTime tempTime = now.AddMinutes(i);
                //    string tempKey = personCode + "Seasky" + tempTime.ToString("yyyyMMddHHmm");
                //    MD5 md5Hash = MD5.Create();
                //    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                //    StringBuilder sBuilder = new StringBuilder();
                //    for (int i = 0; i < data.Length; i++)
                //    {
                //        sBuilder.Append(data[i].ToString("x2"));
                //    }
                //    tempKey = sBuilder.ToString();
                //    if (key.Equals(tempKey))
                //    {
                //        sign = true;
                //    }
                //}
                 
                #endregion

                SHA1 sha1Hash = SHA1.Create();
                byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(tempStr));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                tempStr= sBuilder.ToString();

                if (!tempStr.Equals(sMsgSignature))
                {
                    sign = false;
                }
                log.Debug(_sign + "|PubCore PubAuth:" + sTimeStamp + "-" + sNonce + "-" + sMsgEncrypt);
            }
            catch (Exception err)
            {
                log.Error(_sign + "|PubCore PubAuth:", err);
            }
            return sign;
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
                    type = assembly.GetType("WeChat.PubLib.Model.PubRecEvent" + sEventType.Substring(0, 1).ToUpper() + sEventType.Substring(1).ToLower());
                }
                else
                {
                    type = assembly.GetType("WeChat.PubLib.Model.PubRecMsg" + sMsgType.Substring(0, 1).ToUpper() + sMsgType.Substring(1).ToLower());
                }
                log.Debug(_sign + "|PubCore ReflectClassName:" + type.Name);
                object instance = Activator.CreateInstance(type, new object[] { sMsg });
                if (instance != null)
                {
                    log.Debug(_sign + "|PubCore ProcessMsg instance:" + instance.ToString());
                    PubRecAbstract temp = (PubRecAbstract)instance;
                    sResult = temp.DoProcess();
                    if (string.IsNullOrEmpty(sResult))
                    {
                        sResult = "success";
                    }
                    
                }
            }
            catch (Exception err)
            {
                log.Error(_sign+"|PubCore ProcessMsg:", err);
            }

            return EncryptMsg(pTimeStamp, pNonce, sResult);
        }
        #endregion

        #region 消息处理
        /// <summary>
        /// 消息转发到客服
        /// </summary>
        /// <param name="instanse"></param>
        /// <returns></returns>
        public string TransferCustomerService(PubRecAbstract instanse)
        {
            string strResult = string.Empty;
            if (isCustomerMsg)
            {
                strResult = string.Format(@"<xml><ToUserName>{0}</ToUserName><FromUserName>{1}</FromUserName><CreateTime>1399197672</CreateTime><MsgType><![CDATA[transfer_customer_service]]></MsgType></xml>", instanse.FromUserName,instanse.ToUserName);
            }
            return "success!@!" + strResult;
        }

        /// <summary>
        /// 客服接口-发消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendMsg(PubSendMsgBase msg)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", sAccessToken);
                string result = string.Empty;
                log.Debug(_sign + "|PubCore SendMsg:" + msg.ToJson());
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, msg.ToJson());
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else if ("42001".Equals(jo["errcode"].ToString()))
                {
                    GetAccessToken();
                    SendMsg(msg);
                }
                else
                {
                    log.Info(string.Format(_sign + "|PubCore SendMsg Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error(_sign + "|PubCore SendMsg error!", err);
            }
            return sign;
        }


        /// <summary>
        /// 被动回复消息
        /// </summary>
        /// <param name="instanse"></param>
        /// <returns></returns>
        public string AutoResponse(PubResMsgBase instanse)
        {           
            return "success!@!"+instanse.ToXML(); 
        }
        #endregion

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
                    log.Debug(_sign + "|PubCore DecryptMsg Msg:" + strReuslt);
                    if (ret != 0)
                    {
                        log.Info(_sign + "|PubCore DecryptMsg failed");
                    }
                }
                return strReuslt;
            }
            catch (Exception e)
            {
                log.Error(_sign + "|PubCore DecryptMsg:", e);
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
            string[] tempArray = Regex.Split(postStr,"!@!");
            try
            {
                log.Debug(_sign + "before EncryptMsg:" + strReuslt);
                if (isDES&&(!"success".Equals(tempArray[0])))
                {
                    int ret = 0;
                    ret = wxcpt.EncryptMsg(postStr, sTimeStamp, sNonce,  ref strReuslt);
                    log.Debug(_sign + "|PubCore EncryptMsg Msg:" + strReuslt);
                    if (ret != 0)
                    {
                        log.Info(_sign + "|PubCore EncryptMsg failed");
                    }
                }
                if (tempArray.Length == 2)
                {
                    strReuslt = tempArray[1];
                }
                return strReuslt;
            }
            catch (Exception e)
            {
                log.Error(_sign + "|PubCore EncryptMsg:", e);
                return strReuslt;
            }
        }
        #endregion

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
            log.Info(_sign + "createMenu strjson:" + root.ToJson());
            try
            {  
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}", sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, root.ToJson());

                log.Debug(string.Format(_sign + "CreateMenu result: {0} ", result));
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);

                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format(_sign + "createMenu Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error(_sign + "createMenu Error", err);
            }
            return sign;
        }

        public bool DeleteMenu()
        {
            bool sign = false;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", sAccessToken);
            try
            {
                string result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
            }
            catch (Exception e)
            {
                log.Error(_sign + "DeleteMenu:", e);
            }
            return sign;
        }

        public string GetMenu()
        {
            string strResult = string.Empty;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/get?access_token={0}", sAccessToken);
            try
            {
                strResult = HTTPHelper.GetRequest(url); 
                log.Info(_sign + "GetMenu result:" + strResult);
            }
            catch (Exception e)
            {
                log.Error(_sign + "GetMenu:", e);
            }
            return strResult;
        }

        public bool CreateConditionalMenu(ConditionalRootMenu root)
        {
            bool sign = false;
            string result = string.Empty;
            string strJson = JsonConvert.SerializeObject(root); 
            log.Debug(_sign + "CreateConditionalMenu strjson:" + strJson);
            try
            {  
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/addconditional?access_token={0}",  sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, strJson);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if (jo.Count == 1)
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format(_sign + "createConditionalMenu Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error(_sign + "CreateConditionalMenu Error", err);
            }
            return sign;
        }

        public bool DeleteConditionalMenu(string menuid)
        {
            bool sign = false;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/menu/delete?access_token={0}", sAccessToken);
            try
            {
                string result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
            }
            catch (Exception e)
            {
                log.Error(_sign + "DeleteConditionalMenu:", e);
            }
            return sign;
        }

        #endregion

        #region 二维码处理
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="qrcode">QRCode永久二维码；QRcodeTemp临时二维码</param>
        /// <returns></returns>
        public string createQRCode(QRCode qrcode)
        {
            string result = string.Empty;
            try
            {
                string strJson = JsonConvert.SerializeObject(qrcode);
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token={0}", sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, strJson);
                JObject o = (JObject)JsonConvert.DeserializeObject(result);
                result = o["ticket"].ToString();

            }
            catch (Exception e)
            {
                log.Error("Pub createQRCode error:" ,e);
            }
            return result;
        }


        /// <summary>
        /// 下载图片到本地
        /// </summary>
        /// <param name="picUrl">图片url地址</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="timeOut">等待超时时间，-1不限</param>
        /// <returns></returns>
        public bool DownloadPicture(string picUrl, string savePath, int timeOut)
        {
            bool value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                if (timeOut != -1) request.Timeout = timeOut;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    value = SaveBinaryFile(response, savePath);
            }
            catch (Exception e)
            {
                log.Info("DownloadPicture error" ,e);
            }
            finally
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
            return value;
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="response"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        private  bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath))
                    File.Delete(savePath);
                outStream = System.IO.File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {
                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                }
                while (l > 0);
                value = true;
            }
            catch (Exception e)
            {
                log.Error("SaveBinaryFile error", e);
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }

        #endregion

        #region 模版消息管理
        /// <summary>
        /// 获取模版ID
        /// </summary>
        /// <returns></returns>
        public string GetTemplateID()
        {
            string result = string.Empty;
            try
            { 
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/template/api_add_template?access_token={0}", sAccessToken);
                result = HTTPHelper.GetRequest(url);
            }
            catch (Exception e)
            {
                log.Error(_sign + "getTemplateID error!", e);
            }
            return result;
        }

        /// <summary>
        /// 发送模版信息
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public bool SendTemplate(PubSendMsgTemplate template)
        {
            bool sign = false;
            string result = string.Empty;
            string strJson = JsonConvert.SerializeObject(template);    
            try
            { 
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, strJson);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }

            }
            catch (Exception e)
            {
                log.Error(_sign + "sendTemplate error!", e);
            }
            return sign;
        }

        public enum ServerType
        {
            LocalServer,
            OtherServer
        }
        #endregion

        #region OAuth2.0相关方法

        /// <summary>
        /// 生成OAuth相关的URL
        /// </summary>
        /// <param name="para_URL"></param>
        /// <param name="scope">应用授权作用域，snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        /// <returns></returns>
        public string GetOAuth_URL(string para_URL, ScopeTypeEnum scope, string state)
        {
            return string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect", config.AppID, System.Web.HttpUtility.UrlEncode(para_URL), scope, state);
        }


        /// <summary>
        /// 通过code换取网页授权access_token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public PubOAuth_AccessToken GetOAuth_access_token(string code)
        {
            PubOAuth_AccessToken robject = null;
            if (!string.IsNullOrEmpty(code))
            {
                string result = string.Empty;
                try
                {
                    string url = string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", config.AppID, config.Secret, code);
                    result = HTTPHelper.GetRequest(url); 
                    if (!result.Contains("errcode"))
                    {
                        robject = new PubOAuth_AccessToken(result);
                    }
                    else
                    {
                        log.Info(_sign + "|PubCore getOAuth_access_token Failed! " +result);
                    }
                }
                catch (Exception e)
                {
                    log.Error(_sign + "|PubCore getOAuth_access_token error", e);
                }
            }
            return robject;
        }


        /// <summary>
        /// 由于access_token拥有较短的有效期，当access_token超时后，可以使用refresh_token进行刷新，refresh_token拥有较长的有效期（7天、30天、60天、90天），当refresh_token失效的后，需要用户重新授权。 
        /// </summary>
        /// <param name="refresh_token"></param>
        /// <returns></returns>
        public PubOAuth_AccessToken RrefreshOAuth_access_token(string refresh_token)
        {
            PubOAuth_AccessToken robject = null;
            string result = string.Empty;
            try
            { 
                string url = string.Format("https://api.weixin.qq.com/sns/oauth2/refresh_token?appid={0}&grant_type=refresh_token&refresh_token={1}", config.AppID, refresh_token);
                result = HTTPHelper.GetRequest(url);
                robject = new PubOAuth_AccessToken(result);
            }
            catch (Exception e)
            {
                log.Error(_sign + "|PubCore refreshOAuth_access_token error", e);
            }
            return robject;
        }

        /// <summary>
        /// 检验授权凭证（access_token）是否有效
        /// </summary>
        /// <param name="oauth_accesstoken"></param>
        /// <returns></returns>
        public bool CheckOAuth_access_token(PubOAuth_AccessToken oauth_accesstoken)
        {
            bool sign = false;
            string result = string.Empty;
            try
            { 
                string url = string.Format("https://api.weixin.qq.com/sns/auth?access_token={0}&openid={1}", oauth_accesstoken.access_token, oauth_accesstoken.openid);
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format(_sign + "|PubCore checkOAuth_access_token failed: {0} ", result));
                }

            }
            catch (Exception e)
            {
                log.Error(_sign + "|PubCore checkOAuth_access_token error", e);
            }
            return sign;
        }


        /// <summary>
        /// 如果网页授权作用域为snsapi_userinfo，则此时开发者可以通过access_token和openid拉取用户信息了。 
        /// </summary>
        /// <param name="oauth_accesstoken"></param>
        /// <returns></returns>
        public PubOAuth_UserInfo GetOAuth_UserInfo(PubOAuth_AccessToken oauth_accesstoken)
        {
            PubOAuth_UserInfo temp = null;
            try
            {
                if ("snsapi_userinfo".Equals(oauth_accesstoken.scope.ToLower()))
                {
                    string result = string.Empty;
                    string url = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", oauth_accesstoken.access_token, oauth_accesstoken.openid);
                    result = HTTPHelper.GetRequest(url);
                    if (!result.Contains("errcode"))
                    {
                        temp = new PubOAuth_UserInfo(result);
                    }
                }
                if (temp == null)
                {
                    log.Info(_sign + "|Pubcore getUserInfo result: PubReqPersonInfo is null");
                }
            }
            catch (Exception e)
            {
                log.Error(_sign + "|Pubcore getUserInfo error", e);
            }
            return temp;
        }


        /// <summary>
        /// 应用授权作用域，snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）
        /// </summary>
        public enum ScopeTypeEnum
        {
            snsapi_base,
            snsapi_userinfo
        }
        #endregion

        #region 素材管理
        public string batchget_material(MaterialCondition condition)
        { 
            string result = string.Empty;

            log.Info("batchget_material strjson:" + condition.ToJson());
            try
            {
                string url = string.Format("https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token={0}", sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, condition.ToJson() );
                log.Info(string.Format(_sign + "|Pubcore batchget_material result: {0} ", result));
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
            }
            catch (Exception e)
            {
                log.Error(_sign + "|Pubcore batchget_material Error", e);
            }
            return result;
        }

        
        #endregion

        #region 其他处理
        /// <summary>
        /// 获取两经纬度的距离
        /// </summary>
        /// <param name="loc1_lat">坐标一的纬度</param>
        /// <param name="loc1_lon">坐标一的经度</param>
        /// <param name="loc2_lat">坐标二的纬度</param>
        /// <param name="loc2_lon">坐标二的经度</param>
        /// <returns>两点距离，单位：千米</returns>
        public double getDistance(string loc1_lat, string loc1_lon, string loc2_lat, string loc2_lon)
        {
            double dis = gcdistcalc(double.Parse(loc1_lat), double.Parse(loc1_lon), double.Parse(loc2_lat), double.Parse(loc2_lon));
            return rounda(dis, 1);
        }

        private double gcdistcalc(double loc1_lat, double loc1_lon, double loc2_lat, double loc2_lon)
        {
            double lat1 = loc1_lat * Math.PI / 180;
            double lat2 = loc2_lat * Math.PI / 180;
            double lon1 = -loc1_lon * Math.PI / 180;
            double lon2 = -loc2_lon * Math.PI / 180;

            double distance_rad = 2 * Math.Asin(Math.Sqrt(Math.Pow(Math.Sin((lat1 - lat2) / 2), 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Pow(Math.Sin((lon1 - lon2) / 2), 2)));
            double distance = distance_rad * 60 * 180 / Math.PI * 1.852;

            return distance;
        }

        private double rounda(double num, double place)
        {
            return Math.Round(num * Math.Pow(10, place)) / Math.Pow(10, place);
        }

        /// <summary>
        /// 获取服务器IP段
        /// </summary>
        /// <returns></returns>
        public string getServerIP()
        {
            string result = string.Empty;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/getcallbackip?access_token={0}", sAccessToken);
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                result = jo["ip_list"].ToString();
            }
            catch (Exception e)
            {
                log.Error(_sign + "|Pubcore getServerIP error: {0} ", e);
            }
            return result;
        }
        #endregion
    }
}