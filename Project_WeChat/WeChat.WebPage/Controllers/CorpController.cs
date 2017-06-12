using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WeChat.CorpLib.Core;
using WeChat.WebPage.Base;

namespace WeChat.WebPage.Controllers
{
    public class CorpController : Controller
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static CorpCore core;
        static CookieHelper cookieHelper=new CookieHelper();
        static BasicMethod basicMethod = new BasicMethod();

        string fr_baseURL = ConfigurationManager.AppSettings["fr_baseURL"].ToString();
        string adunit = ConfigurationManager.AppSettings["adunit"].ToString();
        static bool needLogin = bool.Parse(ConfigurationManager.AppSettings["needLogin"].ToString());

        string signMenu = string.Empty;
        string signComp = string.Empty;
        string userId = string.Empty;

        // GET: Corp
        public ActionResult Index(string code, string state)
        {
            //判断state是否为空，初始化CorpCore
            //code = "dZWVZ42Irn0DesLh2IKa_Sdd2KLSnLRE35a8hFFWP4M"; 
            try { 
            if (string.IsNullOrEmpty(state))
            {
                log.Error("Corp/Index error:Lacking of state!");
                Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
            }
            else
            {
                string[] temp = state.Split('!');
                signComp = temp[0].ToString();
                signMenu = temp[1].ToString();
                core = new CorpCore(signComp);
            }

            //获取userId
            if (string.IsNullOrEmpty(code))
            {
                //获取Cookie,减少OAuth2验证频率
                string cookieStr = cookieHelper.getCookie("CorpWechat" + signComp);
                if (string.IsNullOrEmpty(cookieStr))
                {
                    string RedirectURL = core.OAuth_getURL("http://" + Request.Url.Authority.ToString() + "/Corp/Index", CorpCore.ScopeTypeEnum.snsapi_base, state);
                    Response.Redirect(RedirectURL);
                }
                else
                {
                    userId = cookieHelper.DecryptString(cookieStr);
                }
            }
            else
            {
                userId = core.OAuth_getUserInfo(code).UserId;
            }
                //userId = "183725";
            if (string.IsNullOrEmpty(userId))
            {
                log.Error("Corp/Index error:Lacking of userId!");
                Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
            }
            else
            {
                if (needLogin)
                {
                    ViewData["userId"] = userId;
                    ViewData["state"] = state;
                    ViewData["IsValid"] = "True";
                    return View();
                }
                else
                {
                    return RedirectToAction("Login", new { userId = userId, password = "", state = state });
                }
            }
            }
            catch(Exception e)
            {
                log.Error("Corp/Index error", e);
            }
            return View();
        }

        public ActionResult Login(string userId,string password,string state)
        {
            bool sign = false;
            if (string.IsNullOrEmpty(state))
            {
                log.Error("Corp/Login error:Lacking of state!");
                Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
            }
            else
            {
                string[] temp = state.Split('!');
                signComp = temp[0].ToString();
                signMenu = temp[1].ToString(); 
            }
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(password)){
                sign = basicMethod.vertify(userId, password);
                //sign = true;
                if (sign)
                {
                    string url = string.Empty;
                    string key = userId + "SeaskyHR" + DateTime.Now.ToString("yyyyMMddHHmm");

                    //key = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "MD5");
                    MD5 sha1Hash = MD5.Create();
                    byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                    StringBuilder sBuilder = new StringBuilder();
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }
                    key = sBuilder.ToString();

                    if ("salary".Equals(signMenu))
                    {
                        url = string.Format(fr_baseURL, "Salary.cpt&peoplecode=" + userId + "&SalaryYear1=" + DateTime.Now.AddMonths(-5).ToString("yyyy-MM-dd") + "&SalaryYear2=" + DateTime.Now.ToString("yyyy-MM-dd") + "&key=" + key);
                    }
                    else if ("salaryyear".Equals(signMenu))
                    {
                        url = string.Format(fr_baseURL, "Salary_year.cpt&peoplecode=" + userId + "&SalaryYear1=" + DateTime.Now.AddYears(-3).ToString("yyyy") + "&SalaryYear2=" + DateTime.Now.ToString("yyyy-MM-dd") + "&key=" + key);
                    }
                    //MyLog.WriteLog(url);
                    Response.Redirect(url);
                }
                else
                {
                    ViewData["userId"] = userId;
                    ViewData["state"] = state;
                    ViewData["IsValid"] = "false";
                    return View("Index");
                }
            }
            else
            {
                log.Error("Corp/Login error:Lacking of userId or password!");
                Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
            }
            return View();
        }
    }
}