using System;
using System.Web;
using WeChat.PubLib.Core;
using WeChat.PubLib.Menu;
using WeChat.PubLib.Model;
using System.IO;
using System.Text;
using System.Configuration;

namespace WeChat.WebApp
{
    /// <summary>
    /// PubWechatSues 的摘要说明
    /// </summary>
    public class PubWeChatSues : IHttpHandler
    {
        log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        PubCore pubCore;
        static string logoutURL= ConfigurationManager.AppSettings["SuesLogoutURL"];

        public PubWeChatSues()
        {
            pubCore = new PubCore("Sues"); 
             

            PubRecEventClick.OnEventClick += DoClick;
            //PubRecEventSubscribe.OnEventSubscribe += DoSubscribe;
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

                    if (pubCore.PubAuth(pTimeStamp, pNonce, pEchoStr, pMsgSignature))
                    {
                        HttpContext.Current.Response.Write(pEchoStr);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                catch (Exception e)
                {
                    log.Error("ProcessRequest Get:", e);//写入一条新log
                }
            }
        }

        #region 业务逻辑
        public string DoClick(PubRecEventClick instanse)
        {
            string strResult = string.Empty;
            if ("32".Equals(instanse.EventKey))
            {
                
                return pubCore.TransferCustomerService(instanse);
            }
            else if ("31".Equals(instanse.EventKey))
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
                catch(Exception e)
                {
                    log.Error("PubWeChatSues DoClick Logout Err:",e);
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

        public string DoSubscribe(PubRecEventSubscribe instanse)
        {
            log.Info("DoSubscribe");
            return "";
        }

        public string DoMsgText(PubRecMsgText instanse)
        {
            if ("createmenu".Equals(instanse.Content.ToLower()))
            {
                CreateMenu();
            }
            //log.Info("DoMsgText");        
            return "";
        }

  

        public void CreateMenu()
        {
            RootMenu rootmenu = new RootMenu();
            ChildMenu menu1 = new ChildMenu("程财信息");
            ChildMenu menu2 = new ChildMenu("程财服务");
            ChildMenu menu3 = new ChildMenu("程财大厅");

            ChildMenu menu11 = new ChildMenu("项目查询", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/Login/Index?state=SUES!fund");
            ChildMenu menu12 = new ChildMenu("薪资查询", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/Login/Index?state=SUES!salary");
            ChildMenu menu13 = new ChildMenu("来款查询", ChildMenu.MenuTypeEnum.click, "13");
            ChildMenu menu14 = new ChildMenu("报销查询", ChildMenu.MenuTypeEnum.click, "14");
            ChildMenu menu15 = new ChildMenu("学费查询", ChildMenu.MenuTypeEnum.click, "15");

            menu1.sub_button.Add(menu11);
            menu1.sub_button.Add(menu12);
            menu1.sub_button.Add(menu13);
            menu1.sub_button.Add(menu14);
            menu1.sub_button.Add(menu15);

            ChildMenu menu21 = new ChildMenu("报销事务", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/web/1_bxsw.html");
            ChildMenu menu22 = new ChildMenu("办税服务", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/web/2_bsfw.html");
            ChildMenu menu23 = new ChildMenu("薪资服务", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/web/3_xzfw.html");
            ChildMenu menu24 = new ChildMenu("学生事务", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/web/4_xssw.html");
            ChildMenu menu25 = new ChildMenu("公积金事务", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/web/5_gjjsw.html");

            menu2.sub_button.Add(menu21);
            menu2.sub_button.Add(menu22);
            menu2.sub_button.Add(menu23);
            menu2.sub_button.Add(menu24);
            menu2.sub_button.Add(menu25);

            ChildMenu menu31 = new ChildMenu("解除绑定", ChildMenu.MenuTypeEnum.click, "31"); 
            ChildMenu menu32 = new ChildMenu("在线咨询", ChildMenu.MenuTypeEnum.click, "32");
            ChildMenu menu33 = new ChildMenu("学生缴费", ChildMenu.MenuTypeEnum.click, "33");
            ChildMenu menu34 = new ChildMenu("政策法规", ChildMenu.MenuTypeEnum.click, "34");

            menu3.sub_button.Add(menu31);
            menu3.sub_button.Add(menu32);
            menu3.sub_button.Add(menu33);
            menu3.sub_button.Add(menu34);

            rootmenu.button.Add(menu1);
            rootmenu.button.Add(menu2);
            rootmenu.button.Add(menu3);
            pubCore.CreateMenu(rootmenu);
        }
        #endregion
    }
}