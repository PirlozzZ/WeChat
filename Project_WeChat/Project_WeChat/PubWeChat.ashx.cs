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

        public void ProcessRequest(HttpContext context)
        {
            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "Post")
            {

            }
            else
            {
                string pResultStr = "";
                string pMsgSig = HttpContext.Current.Request.QueryString["signature"];
                string pTimeStamp = HttpContext.Current.Request.QueryString["timestamp"];
                string pNonce = HttpContext.Current.Request.QueryString["nonce"];
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"];
                pResultStr = VerifySignature(sVerifyMsgSig, sVerifyTimeStamp, sVerifyNonce, sVerifyEchoStr);
                try
                {
                    WXBizMsgCrypt wxcpt = Config.getWxbizmsgcrypt();
                    int ret = 0;
                    ret = WXBizMsgCrypt.GenarateSinature(sVerifyMsgSig, sVerifyTimeStamp, sVerifyNonce, sVerifyEchoStr, ref sEchoStr);
                    if (ret != 0)
                    {
                        MyLog.WriteLog(string.Format("ERR: VerifyURL fail, ret: {0}", ret));
                    }
                }
                catch (Exception e)
                {
                    MyLog.WriteLog(string.Format("Auth ERR: {0} ", e.Message));
                }
            }
        }

         
    }
}