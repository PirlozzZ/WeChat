using System;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using WeChat.PubLib.Core;
using WeChat.PubLib.Menu;
using WeChat.PubLib.Model;

namespace WeChat.WebApp
{
    /// <summary>
    /// PubWeChat 的摘要说明
    /// </summary>
    public class PubWeChatDsta : IHttpHandler
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static PubCore pubCore;
        static string logoutURL = ConfigurationManager.AppSettings["SuesLogoutURL"];

        static PubWeChatDsta()
        {
            pubCore = new PubCore();
            PubRecEventClick.OnEventClick += DoClick; 
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
            string sResult = "success";

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
                    sResult = pubCore.ProcessMsg(postStr, pMsgSignature, pTimeStamp, pNonce);
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

        #region 业务逻辑
        public static string DoClick(PubRecEventClick instanse)
        {
            string strResult = string.Empty;
            if ("2".Equals(instanse.EventKey))
            {
                try
                {
                    PubResMsgText msg = new PubResMsgText();
                    string flag = HTTPHelper.GetRequest(logoutURL + "?openid=" + instanse.FromUserName);
                    if (bool.Parse(flag))
                    {
                        msg.Content = "解除绑定成功！";
                        msg.CreateTime = instanse.CreateTime;
                        msg.FromUserName = instanse.ToUserName;
                        msg.ToUserName = instanse.FromUserName;
                        strResult = pubCore.AutoResponse(msg);
                    }
                }
                catch (Exception e)
                {
                    log.Error("PubWeChatSues DoClick Logout Err:", e);
                }
            }
            else
            {
                PubResMsgText msg = new PubResMsgText();
                msg.Content = "开发中，敬请期待！";
                msg.CreateTime = instanse.CreateTime;
                msg.FromUserName = instanse.ToUserName;
                msg.ToUserName = instanse.FromUserName;
                strResult = pubCore.AutoResponse(msg);
            }
            return strResult;
        }





        public static string DoMsgText(PubRecMsgText instanse)
        {
            if ("createmenu".Equals(instanse.Content.ToLower()))
            {
                CreateMenu();
            }
            return "";
        }

        public static void CreateMenu()
        {
            RootMenu rootmenu = new RootMenu();
            ChildMenu menu1 = new ChildMenu("信息查询");
            ChildMenu menu2 = new ChildMenu("接触绑定", ChildMenu.MenuTypeEnum.click, "2");

            ChildMenu menu11 = new ChildMenu("项目查询", ChildMenu.MenuTypeEnum.view, "http://wxcw.sta.edu.cn:80/Login/Index?state=SUES!fund");
            ChildMenu menu12 = new ChildMenu("薪资查询", ChildMenu.MenuTypeEnum.view, "http://wxcw.sta.edu.cn:80/Login/Index?state=SUES!salary");


            menu1.sub_button.Add(menu11);
            menu1.sub_button.Add(menu12);
            rootmenu.button.Add(menu1);
            rootmenu.button.Add(menu2);
             

            pubCore.CreateMenu(rootmenu);
        }
        #endregion
    }
}