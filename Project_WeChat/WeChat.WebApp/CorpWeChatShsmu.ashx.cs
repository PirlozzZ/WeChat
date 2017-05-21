using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WeChat.CorpLib.Core;
using WeChat.CorpLib.Model;

namespace WeChat.WebApp
{
    /// <summary>
    /// CorpWeChatShsmu 的摘要说明
    /// </summary>
    public class CorpWeChatShsmu : IHttpHandler
    {

        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static CorpCore corpCore;
        static string logoutURL = ConfigurationManager.AppSettings["ShsmuLogoutURL"];
        static string agentid = ConfigurationManager.AppSettings["ShsmuAgentid"]; 

        static CorpWeChatShsmu()
        {
            corpCore = new CorpCore("Shsmu");
            CorpRecEventClick.OnEventClick += DoClick;
            CorpRecMsgText.OnMsgText += DoMsgText;
        }

        public bool IsReusable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string pMsgSignature = HttpContext.Current.Request.QueryString["msg_signature"];
            string pTimeStamp = HttpContext.Current.Request.QueryString["timestamp"];
            string pNonce = HttpContext.Current.Request.QueryString["nonce"];
            string sResult = "success";



            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                string postStr = string.Empty;
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postStr = Encoding.UTF8.GetString(postBytes);
                }



                log.Debug("ProcessRequest Get:" + postStr);
                if (!string.IsNullOrEmpty(postStr))
                {
                    sResult = corpCore.ProcessMsg(postStr, pMsgSignature, pTimeStamp, pNonce);
                    log.Debug("ProcessRequest sResult:" + sResult);
                }
                HttpContext.Current.Response.Write(sResult);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

            else
            {
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"];
                try
                { 

                    pEchoStr = corpCore.CorpAuth(pTimeStamp, pNonce, pEchoStr, pMsgSignature);
                    log.Debug("ProcessRequest pEchoStr:" + pEchoStr);
                    HttpContext.Current.Response.Write(pEchoStr);
                    HttpContext.Current.ApplicationInstance.CompleteRequest(); 

                }
                catch (Exception e)
                {
                    log.Error("ProcessRequest Get:", e);//写入一条新log
                }
            }
        }

        #region 业务逻辑
        public static string DoClick(CorpRecEventClick instanse)
        {
            string strResult = string.Empty;
            if ("2".Equals(instanse.EventKey))
            {
                try
                {
                    
                    string flag = HTTPHelper.GetRequest(logoutURL + "?openid=" + instanse.FromUserName);
                    if (bool.Parse(flag))
                    {
                        CorpSendMsgText msg = new CorpSendMsgText("解除绑定成功！", instanse.ToUserName,agentid); 
                        corpCore.SendMsg(msg);
                    }
                }
                catch (Exception e)
                {
                    log.Error("PubWeChatDsta DoClick Logout Err:", e);
                }
            }
            else
            { 
                CorpSendMsgText msg = new CorpSendMsgText("开发中，敬请期待！", instanse.ToUserName, agentid);
                corpCore.SendMsg(msg);
            }
            return strResult;
        }





        public static string DoMsgText(CorpRecMsgText instanse)
        {
             
            return "";
        }
         
        #endregion
    }
}