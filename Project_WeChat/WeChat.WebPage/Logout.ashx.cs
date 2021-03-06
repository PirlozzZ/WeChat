﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using WeChat.PubLib.Core;
using WeChat.WebPage.Base;

namespace WeChat.WebPage
{
    /// <summary>
    /// Logout 的摘要说明
    /// </summary>
    public class Logout : IHttpHandler
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器
        static CookieHelper cookieHelper = new CookieHelper();
        public void ProcessRequest(HttpContext context)
        {
            string connStr = ConfigurationManager.AppSettings["WechatDB"].ToString();
            string openid = HttpContext.Current.Request.QueryString["openid"];
            string signComp = HttpContext.Current.Request.QueryString["signComp"];
            try
            {
                if (HttpContext.Current.Request.HttpMethod.ToUpper() == "GET")
                {
                    BasicMethod pb = new BasicMethod();
                    bool flag = pb.DeleteData(openid);
                    cookieHelper.delCookie("PubWechat" + signComp);
                    log.Debug("Logout delete openid" + openid);
                    HttpContext.Current.Response.Write(flag);
                    HttpContext.Current.Response.Flush();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
            }
            catch (Exception e)
            {
                log.Error("Logout err! openid:" + openid, e);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}