using Project_WeChat.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tencent;

namespace Project_WeChat
{
    /// <summary>
    /// PubWeChat 的摘要说明
    /// </summary>
    public class PubWeChat : IHttpHandler
    {
        log4net.ILog log_err = log4net.LogManager.GetLogger("Error.Logging");//获取一个日志记录器
        log4net.ILog log_deb = log4net.LogManager.GetLogger("Debug.Logging");//获取一个日志记录器

        public bool IsReusable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "Post")
            {

            }
            else
            {
                log_deb.Info("init test"); 
                string sMsgSignature = HttpContext.Current.Request.QueryString["signature"];
                string pTimeStamp = HttpContext.Current.Request.QueryString["timestamp"];
                string pNonce = HttpContext.Current.Request.QueryString["nonce"];
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"]; 
                try
                { 
                    bool result = false;
                    result = PubCore.PubAuth(pTimeStamp, pNonce, pEchoStr, sMsgSignature);
                    if (result)
                    { 
                        HttpContext.Current.Response.Write("success");
                        HttpContext.Current.Response.End();
                    }
                }
                catch (Exception e)
                {

                    log_err.Info(DateTime.Now.ToString("yyyy-MM-dd") + ": login success");//写入一条新log
                }
            }
        }

         
    }
}