using System.Web.Mvc;

namespace WeChat.WebPage.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index(string para)
        {
            ViewData["openID"] = para;
            return View();
        }
    }
}