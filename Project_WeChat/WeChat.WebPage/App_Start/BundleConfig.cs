using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace WeChat.WebPage
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts").Include(
                        "~/Scripts/jquery-1.*"));
            bundles.Add(new StyleBundle("~/Css").Include(
                        "~/Css/weui.css"));
        }
    }
}