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
        private DateTime sDateTime { get;  set; }
        private string _sAccessToken;
        private string sAccessToken
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
            set { _sAccessToken = value; }
        }
        private Config config;
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");
        bool isDES = bool.Parse(ConfigurationManager.AppSettings["isDES"]);
        bool isCustomerMsg = bool.Parse(ConfigurationManager.AppSettings["isCustomerMsg"]);
        WXBizMsgCrypt wxcpt;

        #region 构造方法
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
        public bool CreateUser(CorpUser user)
        {
            bool sign = false;
            try
            {
                string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}", sAccessToken);
                string result = string.Empty;
                result = HTTPHelper.PostRequest(url, DataTypeEnum.json, user.ToJson());
                JObject jo = (JObject)JsonConvert.DeserializeObject(result);
                if ("created".Equals(jo["errmsg"].ToString()))
                {
                    sign = true;
                }
                else
                {
                    log.Info(string.Format("CorpCore CreateUser Failed: {0} ", result));
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
