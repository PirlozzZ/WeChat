using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WeChat.PubLib.Core;
using WeChat.PubLib.Menu;
using WeChat.PubLib.Model;

namespace WeChat.WebApp
{
    /// <summary>
    /// PubWeChatSta 的摘要说明
    /// </summary>
    public class PubWeChatSta : IHttpHandler
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static PubCore pubCoreSta;
        static string logoutURL = ConfigurationManager.AppSettings["StaLogoutURL"];

        static PubWeChatSta()
        {
            pubCoreSta = new PubCore("Sta");
            PubRecEventClick.OnEventClick += DoClick;
            PubRecMsgText.OnMsgText += DoMsgText;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            string pMsgSignature = HttpContext.Current.Request.QueryString["signature"];
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

                log.Debug("ProcessRequestSta Get:" + postStr);
                if (!string.IsNullOrEmpty(postStr))
                {
                    sResult = pubCoreSta.ProcessMsg(postStr, pMsgSignature, pTimeStamp, pNonce);
                    log.Debug("ProcessRequestSta sResult:" + sResult);
                }
                HttpContext.Current.Response.Write(sResult);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

            else
            {
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"];
                string seaskyAccessToken = HttpContext.Current.Request.QueryString["seaskyAccessToken"];
                try
                {
                    if ("Sta".Equals(seaskyAccessToken))
                    {
                        HttpContext.Current.Response.Write(pubCoreSta.sAccessToken);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    else if (pubCoreSta.PubAuth(pTimeStamp, pNonce, pEchoStr, pMsgSignature))
                    {
                        HttpContext.Current.Response.Write(pEchoStr);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                catch (Exception e)
                {
                    log.Error("ProcessRequestSta Get:", e);//写入一条新log
                }
            }
        }

        #region 业务逻辑
        public static string DoClick(PubRecEventClick instanse)
        {
            string strResult = string.Empty;
            if ("31".Equals(instanse.EventKey))
            {
                try
                {
                    string flag = HTTPHelper.GetRequest(logoutURL + "?openid=" + instanse.FromUserName+ "&signComp=Sta");
                    if (bool.Parse(flag))
                    {
                        PubSendMsgText msg = new PubSendMsgText("解除绑定成功！", instanse.FromUserName);
                        pubCoreSta.SendMsg(msg);
                    }
                }
                catch (Exception e)
                {
                    log.Error("PubWeChatSta DoClick Logout Err:", e);
                }
            }
            else
            {
                PubSendMsgText msg = new PubSendMsgText("开发中，敬请期待！！", instanse.FromUserName);
                pubCoreSta.SendMsg(msg);
            }
            return strResult;
        }



        public static string DoMsgText(PubRecMsgText instanse)
        {
            PubResMsgText msg = new PubResMsgText();
            string strResult = string.Empty;
            if ("createmenu".Equals(instanse.Content.ToLower()))
            {
                CreateMenu();
            }

            //log.Info("DoMsgText");        
            return strResult;
        }

        public static void CreateMenu()
        {
            #region 菜单测试 
            RootMenu rootmenu = new RootMenu();

            ChildMenu menu1 = new ChildMenu("信息查询");
            ChildMenu menu2 = new ChildMenu("业务办理");
            ChildMenu menu3 = new ChildMenu("用户服务");

            ChildMenu menu11 = new ChildMenu("薪资查询", ChildMenu.MenuTypeEnum.view, "http://Cwcw.sta.edu.cn/Pub/Index?state=Sta!salary");
            ChildMenu menu12 = new ChildMenu("项目查询", ChildMenu.MenuTypeEnum.view, "http://Cwcw.sta.edu.cn/Pub/Index?state=Sta!fund");
            ChildMenu menu13 = new ChildMenu("学费查询", ChildMenu.MenuTypeEnum.view, "http://Cwcw.sta.edu.cn/Pub/Index?state=Sta!charge");
            ChildMenu menu14 = new ChildMenu("来款查询", ChildMenu.MenuTypeEnum.click, "14");
            ChildMenu menu15 = new ChildMenu("津补贴查询", ChildMenu.MenuTypeEnum.click, "15");


            menu1.sub_button.Add(menu11);
            menu1.sub_button.Add(menu12);
            menu1.sub_button.Add(menu13);
            menu1.sub_button.Add(menu14);
            menu1.sub_button.Add(menu15);

            ChildMenu menu21 = new ChildMenu("学生缴费", ChildMenu.MenuTypeEnum.view, "http://payment.cloud.ccb.com/index_u.jhtml?param=4C4A647F371B43F11DBA91BA7799EE58C59D79B9E834A07D7CE6C5522E5A9F46DA6A3FC12483D94B4D562A556E153CFFD15776F0B17D91BE65EC5221B4A931F1C1F6331EE5E1ED11232A46AA1E196B49DC7901A2E97F9D32C9472BFD4CA567687AC7760BB7E6CB52E11236DC3E5F61C1D1B21F50E66048EA47A4526B703C5C90546B9B12CB61C99D9264D07D287F5EC8");
            ChildMenu menu22 = new ChildMenu("报账跟踪", ChildMenu.MenuTypeEnum.click, "22");
            

            menu2.sub_button.Add(menu21);
            menu2.sub_button.Add(menu22);

            ChildMenu menu31 = new ChildMenu("通知公告", ChildMenu.MenuTypeEnum.view, "http://cwcw.sta.edu.cn:8001/Home/Articles?m=0c356049-28f7-475f-98fa-152d51737ed5");
            ChildMenu menu32 = new ChildMenu("报销制度", ChildMenu.MenuTypeEnum.view, "http://cwcw.sta.edu.cn/bxzn.html");
            ChildMenu menu33 = new ChildMenu("发票验真", ChildMenu.MenuTypeEnum.view, "https://www.tax.sh.gov.cn/wsbs/WSBSptFpCx_loginsNewl.jsp");
            ChildMenu menu34 = new ChildMenu("解除绑定", ChildMenu.MenuTypeEnum.click, "31");
            menu3.sub_button.Add(menu31);
            menu3.sub_button.Add(menu32);
            menu3.sub_button.Add(menu33);
            menu3.sub_button.Add(menu34);

            rootmenu.button.Add(menu1);
            rootmenu.button.Add(menu2);
            rootmenu.button.Add(menu3);
            pubCoreSta.CreateMenu(rootmenu);
            #endregion
        }
        #endregion
    }
}