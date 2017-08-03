using System;
using System.Configuration;
using System.Web.Mvc;
using WeChat.PubLib.Core;
using WeChat.WebPage.Base;

namespace WeChat.WebPage.Controllers
{
    public class PubController : Controller
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static PubCore core;
        static CookieHelper cookieHelper = new CookieHelper();
        static BasicMethod basicMethod = new BasicMethod();

        string fr_baseURL = ConfigurationManager.AppSettings["fr_baseURL"].ToString();
        string adunit = ConfigurationManager.AppSettings["adunit"].ToString();
        static bool needLogin = bool.Parse(ConfigurationManager.AppSettings["needLogin"].ToString());

        string signMenu = string.Empty;
        string signComp = string.Empty;
        
        // GET: Pub
        public ActionResult Index(string code, string state)
        {
            string openID = string.Empty;
            string userID = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(state))
                { 
                    log.Error("Pub/Index error:Lacking of state!");
                    Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
                }
                else
                {
                    string[] temp = state.Split('!');
                    signComp = temp[0].ToString();
                    signMenu = temp[1].ToString();
                    core = new PubCore(signComp);
                }

                //获取userId
                if (string.IsNullOrEmpty(code))
                {
                    //获取Cookie,减少OAuth2验证频率
                    string cookieStr = cookieHelper.getCookie("PubWechat" + signComp);
                    if (string.IsNullOrEmpty(cookieStr))
                    {
                        string RedirectURL = core.GetOAuth_URL("http://" + Request.Url.Authority.ToString() + "/Pub/Index", PubCore.ScopeTypeEnum.snsapi_base, state);
                         
                        Response.Redirect(RedirectURL);
                    }
                    else
                    {
                        openID = cookieHelper.DecryptString(cookieStr);
                    }
                }
                else
                {
                    openID = core.GetOAuth_access_token(code).openid;
                }
                //userId = "183725";
                
                if (string.IsNullOrEmpty(openID))
                {
                    
                    log.Error("Pub/Index error:Lacking of openid!");
                    Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
                }
                else
                { 
                    userID = basicMethod.getLoginno(openID);
                    
                    if (needLogin||string.IsNullOrEmpty(userID))
                    {
                        ViewData["userID"] = userID;
                        ViewData["openID"] = openID;
                        ViewData["state"] = state;
                        ViewData["IsValid"] = "True";
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Login", new { userID = userID, password = "htP@ssw0rd", state = state,openID=openID });
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Pub/Index error", e);
            }
            return View();
        }

        public ActionResult Login(string userID, string password, string state,string openID)
        {
            log.Info("temp debug:" + userID+password+openID);
            bool sign = false;
            if (string.IsNullOrEmpty(state))
            {
                log.Error("Pub/Login error:Lacking of state!");
                Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
            }
            else
            {
                string[] temp = state.Split('!');
                signComp = temp[0].ToString();
                signMenu = temp[1].ToString();
            }
            if (!string.IsNullOrEmpty(userID) && !string.IsNullOrEmpty(password))
            {
                password = "htP@ssw0rd".Equals(password) ? "" : password;
                //for debug
                //userId = "arogornl".Equals(userId.ToLower()) ? "2000900301" : userId;
                sign = basicMethod.vertify(userID, password, openID);
                //sign = true;
                log.Debug("vertify result:" + sign);
                if (sign)
                {

                    string url = string.Empty;
                    //string key = userId + "SeaskyHR" + DateTime.Now.ToString("yyyyMMddHHmm");

                    cookieHelper.delCookie("PubWechat" + signComp);
                    cookieHelper.setCookie("PubWechat" + signComp, cookieHelper.EncryptString(userID), 2);

                    //key = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(key, "MD5");
                    //MD5 sha1Hash = MD5.Create();
                    //byte[] data = sha1Hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                    //StringBuilder sBuilder = new StringBuilder();
                    //for (int i = 0; i < data.Length; i++)
                    //{
                    //    sBuilder.Append(data[i].ToString("x2"));
                    //}
                    //key = sBuilder.ToString();

                    if ("salary".Equals(signMenu))
                    {
                        url = string.Format(fr_baseURL, "Salary_New.cpt&peoplecode=" + openID);
                    }
                    else if ("fund".Equals(signMenu))
                    {
                        url = string.Format(fr_baseURL, "Fund_New.cpt&peoplecode=" + openID);
                    }
                    else if ("charge".Equals(signMenu))
                    {
                        url = string.Format(fr_baseURL, "Charge_New.cpt&peoplecode=" + openID);
                    }
                    //MyLog.WriteLog(url);
                    Response.Redirect(url);
                }
                else
                {
                    ViewData["userId"] = userID;
                    ViewData["state"] = state;
                    ViewData["IsValid"] = "false";
                    return View("Index");
                }
            }
            else
            {
                log.Error("Pub/Login error:Lacking of userId or password!");
                Response.Redirect("http://" + Request.Url.Authority.ToString() + "/Error.htm");
            }
            return View();
        }
    }
}