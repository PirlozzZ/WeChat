using Project_WeChat.Core;
using Project_WeChat.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            string sMsgSignature = HttpContext.Current.Request.QueryString["signature"];
            string pTimeStamp = HttpContext.Current.Request.QueryString["timestamp"];
            string pNonce = HttpContext.Current.Request.QueryString["nonce"];

            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "Post")
            {
                string postStr = string.Empty;
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postStr = Encoding.UTF8.GetString(postBytes);
                }
                if (!string.IsNullOrEmpty(postStr))
                {
                    Execute(postStr, sMsgSignature, pTimeStamp, pNonce);

                }
            }
            else
            { 
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"]; 
                try
                {
                    //for debug
                    //pTimeStamp = "1483334816";
                    //pNonce = "709329334";
                    //pEchoStr = "6890308849856673530";
                    //sMsgSignature = "7ebda6ce61cbd4f8fbbca69e195c2768e3c9e71e";

                    if (pubCore.PubAuth(pTimeStamp, pNonce, pEchoStr, sMsgSignature))
                    { 
                        HttpContext.Current.Response.Write(pEchoStr);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        //HttpContext.Current.Response.End(); 
                    }
                }
                catch (Exception e)
                {

                    log.Error("ProcessRequest Get:",e);//写入一条新log
                }
            }
        }

        private void Execute(string postStr, string sMsgSignature, string pTimeStamp, string pNonce)
        {
            bool sign = true;
            string result = "success";
            string sMsgType = string.Empty;
            string sEventType = string.Empty;
            //string sMsg = core.decryptMsg(sReqMsgSig, sReqTimeStamp, sReqNonce, postStr, ref sMsgType, ref sEventType);  // 解析之后的明文
            //PubBaseEvent be = null;
            //MyLog.WriteLog("temp sMsg:" + sMsg + sMsgType.ToLower() + sEventType.ToLower());
            #region 响应事件消息
            //if ("event".Equals(sMsgType.ToLower()))
            //{
            //    if ("click".Equals(sEventType.ToLower()))
            //    {
            //        be = new PubEventClick(sMsg);
            //        sign = doClick(be, out result);
            //    }
            //} 
            #endregion
        }
    }
}