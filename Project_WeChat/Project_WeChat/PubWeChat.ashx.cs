using Project_WeChat.Core;
using Project_WeChat.Menu;
using Project_WeChat.Model;
using System;
using System.IO;
using System.Text;
using System.Web;

namespace Project_WeChat
{
    /// <summary>
    /// PubWeChat 的摘要说明
    /// </summary>
    public class PubWeChat : IHttpHandler
    {
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        PubCore pubCore = new PubCore();

        public PubWeChat()
        {
            PubRecEventClick.OnEventClick += DoClick;
            PubRecEventSubscribe.OnEventSubscribe += DoSubscribe;
            PubRecMsgText.OnMsgText += DoMsgText;
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
            string pMsgSignature = HttpContext.Current.Request.QueryString["signature"];
            string pTimeStamp = HttpContext.Current.Request.QueryString["timestamp"];
            string pNonce = HttpContext.Current.Request.QueryString["nonce"];

            #region for debug
            //pTimeStamp = "1483334816";
            //pNonce = "709329334";
            //pMsgSignature = "7ebda6ce61cbd4f8fbbca69e195c2768e3c9e71e";
            #endregion

            if (HttpContext.Current.Request.HttpMethod.ToUpper() == "POST")
            {
                string postStr = string.Empty;
                using (Stream stream = HttpContext.Current.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postStr = Encoding.UTF8.GetString(postBytes);
                }

                #region for debug 
                //postStr = "<xml><ToUserName><![CDATA[gh_dc9f5aee123b]]></ToUserName><FromUserName><![CDATA[o6w0juLpCRYHJTqUTcG1L9hz-uh0]]></FromUserName><CreateTime>1484057461</CreateTime><MsgType><![CDATA[text]]></MsgType><Content><![CDATA[6]]></Content><MsgId>6373978260806390997</MsgId><Encrypt><![CDATA[dK8jouN8itPoAPz+kN1ayvWSKlKQsyEOA2A9ja0Vnq/kdsHnQgWlKYt5HAqkUwWIs2B09BVUkZuvlYgsYar6VcjjuH2kPtFbfhHYmYDKASVDEq2y4/ZLiMhdYm9aWIoTlsYTPWEuD8MDYbWmDSfFKbC5N39XbO38yqifgkHWsA9f7/NuiWTczjw4CjsjpwjlciZwPWo52YiprcafwwtQcYia+jCAyhkaaezAqFcSIchYhBQQw0RfHFb0Ig6ved8Dxw/jWqZEjAonfOiPpZiRBz0y2OvjxSIWroHAqLk9qPESA2zAW09F+HM/gX/PAmmXAvnoGn95Cgku2gv+IcF0xE1d8o8UWneRrOTMspriI9Wg01fanLK+kUQK25cfMPLjF0EZlzEoSSzZIzarKc4KefBLWRpPbX07NQYfZxT5T6Q=]]></Encrypt></xml>";
#endregion

                log.Debug("ProcessRequest Get:" + postStr);
                if (!string.IsNullOrEmpty(postStr))
                {
                    if (!pubCore.ProcessMsg(postStr, pMsgSignature, pTimeStamp, pNonce))
                    {
                        log.Info("PubCore ProcessMsg Failed!");
                    }
                    HttpContext.Current.Response.Write("success");
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            else
            {
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"];
                try
                {
                    #region for debug
                    //pTimeStamp = "1483334816";
                    //pNonce = "709329334";
                    //pEchoStr = "6890308849856673530";
                    //pMsgSignature = "7ebda6ce61cbd4f8fbbca69e195c2768e3c9e71e";
                    #endregion

                    if (pubCore.PubAuth(pTimeStamp, pNonce, pEchoStr, pMsgSignature))
                    {
                        HttpContext.Current.Response.Write(pEchoStr);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        //HttpContext.Current.Response.End(); 
                    }
                }
                catch (Exception e)
                {
                    log.Error("ProcessRequest Get:", e);//写入一条新log
                }
            }
        }

        public void DoClick(PubRecEventClick instanse)
        {
            log.Info("DoClick");
        }

        public void DoSubscribe(PubRecEventSubscribe instanse)
        {
            log.Info("DoSubscribe");
        }

        public void DoMsgText(PubRecMsgText instanse)
        {
            if ("createmenu".Equals(instanse.Content.ToLower()))
            {
                CreateMenu();
            }
        }

        public void CreateMenu()
        {
            ConditionalRootMenu rootmenu = new ConditionalRootMenu();
            ChildMenu menu1 = new ChildMenu("菜单女一");
            ChildMenu menu2 =new ChildMenu("菜单二", ChildMenu.MenuTypeEnum.click,"2");

            ChildMenu menu11 = new ChildMenu("子菜单一", ChildMenu.MenuTypeEnum.click, "11");
            ChildMenu menu12 = new ChildMenu("子菜单二", ChildMenu.MenuTypeEnum.view, "http://www.baidu.com");

            menu1.sub_button.Add(menu11);
            menu1.sub_button.Add(menu12);
            rootmenu.button.Add(menu1);
            rootmenu.button.Add(menu2);

            rootmenu.matchrule.sex = "2";

            pubCore.CreateMenu(rootmenu);
        }
    }
}