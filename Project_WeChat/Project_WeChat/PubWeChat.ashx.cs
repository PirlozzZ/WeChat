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
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        PubCore pubCore = new PubCore();

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
                string sMsgSignature = HttpContext.Current.Request.QueryString["signature"];
                string pTimeStamp = HttpContext.Current.Request.QueryString["timestamp"];
                string pNonce = HttpContext.Current.Request.QueryString["nonce"];
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"]; 
                try
                { 
                    string sEchoStr = string.Empty;
                    sEchoStr = pubCore.PubAuth(pTimeStamp, pNonce, pEchoStr, sMsgSignature);
                    if (!string.IsNullOrEmpty(sEchoStr))
                    {
                        HttpContext.Current.Response.Write(sEchoStr);
                        HttpContext.Current.Response.End();
                    }
                }
                catch (Exception e)
                {

                    log.Error("ProcessRequest Get:",e);//写入一条新log
                }
            }
        }

         
    }
}