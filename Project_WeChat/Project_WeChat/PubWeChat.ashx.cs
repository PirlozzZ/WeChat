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

                string pResultStr = "";
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
                        pResultStr = "success";
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

         
    }
}