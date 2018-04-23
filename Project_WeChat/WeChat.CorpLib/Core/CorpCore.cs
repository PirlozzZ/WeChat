using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using Tencent;
using System.Xml;
using System.Reflection;
using WeChat.CorpLib.Model;
using System.Collections.Generic;

namespace WeChat.CorpLib.Core
{
    public class CorpCore
    {
        //排重集合
        private List<string> list;

        private DateTime sDateTime { get;  set; }
        private string _sAccessToken;
        private string sAccessToken
        {
            get
            {
                DateTime temp = DateTime.Now;
                TimeSpan timespan = temp - sDateTime;
                if (timespan.TotalMilliseconds >= config.expires_in)
                {
                    GetAccessToken();
                    sDateTime = temp;
                }
                return _sAccessToken;
            }
            set { _sAccessToken = value; }
        }
        private Config config;
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");
        bool isDES ; 
        WXBizMsgCrypt wxcpt;

        #region 构造方法
        public CorpCore() : this("")
        {

        }

        public CorpCore(string sign)
        { 
            config = new Config(sign);
            sDateTime = DateTime.Now;
            isDES = bool.Parse(ConfigurationManager.AppSettings[sign + "isDES"]);
            list =  new List<string>();
            //isCustomerMsg = bool.Parse(ConfigurationManager.AppSettings[sign + "isCustomerMsg"]);
            if (isDES)
            {
                wxcpt = new WXBizMsgCrypt(config.Token, config.EncodingAESKey, config.AppID);
            }
            if (sAccessToken == null)
                GetAccessToken();
        }
        #endregion

        #region OAuth2.0相关处理

        /// <summary>
        /// 应用授权作用域。
        ///snsapi_base：静默授权，可获取成员的基础信息；
        ///snsapi_userinfo：静默授权，可获取成员的详细信息，但不包含手机、邮箱；
        ///snsapi_privateinfo：手动授权，可获取成员的详细信息，包含手机、邮箱。
        /// </summary>
        public enum ScopeTypeEnum
        {
            snsapi_base,
            snsapi_userinfo,
            snsapi_privateinfo
        }

        /// <summary>
        /// 生成OAuth相关的URL
        /// </summary>
        /// <param name="para_URL"></param>
        /// <param name="scope">应用授权作用域
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        /// <returns></returns>
        public string GetOAuth_URL(string para_URL, ScopeTypeEnum scope, string state)
        {
            string OAuth_URL = string.Empty;
            try
            { 
                OAuth_URL = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect", config.AppID, System.Web.HttpUtility.UrlEncode(para_URL),scope.ToString(), state);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Corp getOAuth_URL:"),e);
            }
            return OAuth_URL;
        }

        /// <summary>
        /// OAuth根据Code获取用户code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="agentid"></param>
        /// <returns></returns>
        public CorpOAuth_UserInfo GetOAuth_UserInfo(string code)
        {
            CorpOAuth_UserInfo instance = null;
            string result = string.Empty;
            if (!string.IsNullOrEmpty(code))
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}", sAccessToken, code);   
                try
                {
                    result = HTTPHelper.GetRequest(url);
                    instance = JsonConvert.DeserializeObject<CorpOAuth_UserInfo>(result);
                    if (instance != null)
                    {
                        if (!"0".Equals(instance.errcode))
                        {
                            log.Info(string.Format("CorpOAuth_getUserInfo Failed:{0}", instance.errcode + instance.errmsg+"--"+url));
                        }
                        else
                        {
                            log.Debug(string.Format("CorpOAuth_getUserInfo success", instance.UserId));
                        }
                    }
                    else
                    {
                        log.Info(string.Format("CorpOAuth_getUserInfo JsonConvert Failed:{0}", result));
                    }
                }
                catch (Exception e)
                {
                    log.Error(string.Format("CorpOAuth_getUserInfo ERR:{0}",url), e);
                }
            }
            return instance;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="Employeecode">微信帐号号码</param>
        /// <returns>用户信息</returns>
        public CorpOAuth_UserDetail GetOAuth_UserDetail(string user_ticket)
        {
            CorpOAuth_UserDetail instance = null;
            string result = string.Empty;
            try
            { 
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserdetail?access_token={0}", sAccessToken);
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, string.Format("{\"user_ticket\": \"{0}\"}", user_ticket));
                instance = JsonConvert.DeserializeObject<CorpOAuth_UserDetail>(result);
            }
            catch (Exception e)
            {
                log.Error(string.Format("Corp OAuth_getUserDetail ERR!"), e);
            }
            return instance;
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

        /// <summary>
        /// 服务器验证
        /// </summary>
        /// <param name="sTimeStamp"></param>
        /// <param name="sNonce"></param>
        /// <param name="sEchoStr"></param>
        /// <param name="sMsgSignature"></param>
        /// <returns></returns>
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
                log.Error("CorpAuth error:"+sTimeStamp+"--"+sNonce + "--" +sEchoStr + "--" +sMsgSignature, e); 
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
                    type = assembly.GetType("WeChat.CorpLib.Model.CorpRecEvent" + sEventType.Substring(0, 1).ToUpper() + sEventType.Substring(1).ToLower());
                }
                else
                {
                    type = assembly.GetType("WeChat.CorpLib.Model.CorpRecMsg" + sMsgType.Substring(0, 1).ToUpper() + sMsgType.Substring(1).ToLower());
                } 
                log.Debug("CorpCore ReflectClassName:" + type.Name);
                object instance = Activator.CreateInstance(type, new object[] { sMsg }); 
                if (instance != null)
                {
                    CorpRecAbstract temp = (CorpRecAbstract)instance;

                    //排重处理，同一个用户同一个创建时间只响应一次
                    if (list.Contains(temp.FromUserName + temp.CreateTime)) 
                    {
                        list.RemoveAll(x=>x.StartsWith(temp.FromUserName));
                    }
                    else{
                        list.Add(temp.FromUserName + temp.CreateTime);
                        sResult = temp.DoProcess();
                    }
                    
                    if (string.IsNullOrEmpty(sResult))
                    {
                        sResult = "";
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
        #endregion

        #region 消息发送
        /// <summary>
        /// 普通消息发送
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendMsg(CorpSendMsgBase msg)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}", sAccessToken);
                string result = string.Empty;
                msg.agentid = config.agientID;
                log.Debug("CorpCore SendMsg:"+msg.ToJson());
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, msg.ToJson());
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    log.Debug("CorpCore SendMsg Result:"+jo.ToString());
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore SendMsg Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore SendMsg error!", err);
            }
            return sign;
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
                    log.Debug("CorpCore DecryptMsg Msg:" + strReuslt);
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

        #region 企业号部门维护
        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns>创建的部门id</returns>
        public string CreateDepartment(CorpDepartment dept)
        {
            string sign = string.Empty;
            try
            { 
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/create?access_token={0}", sAccessToken);
                string result = string.Empty;
                result = HTTPHelper.PostRequest(url,DataTypeEnum.json,dept.ToJson() ); 
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("created".Equals(jo["errmsg"].ToString()))
                {
                    sign = jo["id"].ToString();
                }
                else
                {
                    log.Info(string.Format("CorpCore CreateDepartment Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore CreateDepartment error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="dept"></param>
        /// <returns>更新结果</returns>
        public bool UpdateDepartment(CorpDepartment dept)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/update?access_token={0}", sAccessToken);
                string result = string.Empty;
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, dept.ToJson());
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("updated".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore UpdateDepartment Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore UpdateDepartment error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">删除部门id</param>
        /// <returns>删除结果</returns>
        public bool DeleteDepartment(string id)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/delete?access_token={0}&id={1}", sAccessToken,id);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("deleted".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore DeleteDepartment Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore DeleteDepartment error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <param name="id">部门id。获取指定部门及其下的子部门</param>
        /// <returns>返回指定部门及其下的子部门的集合</returns>
        public List<CorpDepartment> ListDepartment(string id)
        {
            List<CorpDepartment> list = new List<CorpDepartment>();
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={0}&id={1}", sAccessToken, id);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                { 
                    list= JsonConvert.DeserializeObject<List<CorpDepartment>>(jo["department"].ToString());
                }
                else
                {
                    log.Info(string.Format("CorpCore ListDepartment Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore ListDepartment error!", err);
            }
            return list;
        }
        #endregion

        #region 企业号成员维护
        /// <summary>
        /// 创建成员
        /// </summary>
        /// <param name="user"></param>
        /// <returns>创建结果</returns>
        public string CreateUser(CorpUser user)
        {
            string sign = string.Empty;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}", sAccessToken);
                string result = string.Empty;
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, user.ToJson());
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("created".Equals(jo["errmsg"].ToString()))
                {
                    sign = "success";
                }
                else
                {
                    log.Info(string.Format("CorpCore CreateUser Failed: {0} ", result));
                    sign = jo["errcode"].ToString();
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore CreateUser error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 更新成员
        /// </summary>
        /// <param name="user"></param>
        /// <returns>更新结果</returns>
        public bool UpdateUser(CorpUser user)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/update?access_token={0}", sAccessToken);
                string result = string.Empty;
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, user.ToJson());
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("updated".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore UpdateUser Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore UpdateUser error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 删除成员
        /// </summary>
        /// <param name="userid">成员UserID。对应管理端的帐号</param>
        /// <returns>删除结果</returns>
        public bool DeleteUser(string userid)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/delete?access_token={0}&userid={1}", sAccessToken, userid);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("deleted".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore DeleteUser Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore DeleteUser error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 批量删除成员
        /// </summary>
        /// <param name="id">成员UserID。对应管理端的帐号</param>
        /// <returns>删除结果</returns>
        public bool BatchDeleteUser(string[] useridlist)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete?access_token={0}", sAccessToken);
                string result = string.Empty;
                result = HTTPHelper.PostRequest(url,DataTypeEnum.json, JsonConvert.SerializeObject(useridlist));
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("deleted".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore BatchDeleteUser Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore BatchDeleteUser error!", err);
            }
            return sign;
        }

        /// <summary>
        /// 获取成员
        /// </summary>
        /// <param name="userid">成员UserID。对应管理端的帐号</param>
        /// <returns></returns>
        public CorpUser GetUser(string userid)
        {
            CorpUser user = new CorpUser();
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}", sAccessToken, userid);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    user = jo.ToObject<CorpUser>();
                }
                else
                {
                    log.Info(string.Format("CorpCore GetUser Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore GetUser error!", err);
            }
            return user;
        }



        /// <summary>
        /// 获取部门成员(详情)
        /// </summary>
        /// <param name="department_id">获取的部门id</param>
        /// <param name="fetch_child">	1/0：是否递归获取子部门下面的成员</param>
        /// <param name="status">0获取全部成员，1获取已关注成员列表，2获取禁用成员列表，4获取未关注成员列表。status可叠加,未填写则默认为4</param>
        /// <returns></returns>
        public List<CorpUser> ListUser(string department_id, string fetch_child,string status)
        {
            List<CorpUser> list = new List<CorpUser>();
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/list?access_token={0}&department_id={1}&fetch_child={2}&status={3}", sAccessToken, department_id,fetch_child,status);
                string result = string.Empty;
                result = HTTPHelper.GetRequest(url);
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("ok".Equals(jo["errmsg"].ToString()))
                {
                    list = JsonConvert.DeserializeObject<List<CorpUser>>(jo["userlist"].ToString());
                }
                else
                {
                    log.Info(string.Format("CorpCore ListUser Failed: {0} ", result));
                }
            }
            catch (Exception err)
            {
                log.Error("CorpCore ListUser error!", err);
            }
            return list;
        }
        #endregion
    }
}
