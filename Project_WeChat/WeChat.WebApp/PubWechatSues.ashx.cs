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
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static PubCore pubCore;
        static string logoutURL= ConfigurationManager.AppSettings["SuesLogoutURL"]; 

        static PubWeChatSues()
        { 
            pubCore = new PubCore("Sues");            
            PubRecEventClick.OnEventClick += DoClick;
            PubRecEventSubscribe.OnEventSubscribe += DoSubscribe;        
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
        public static string DoClick(PubRecEventClick instanse)
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
                //PubResMsgText msg = new PubResMsgText();
                //msg.Content = "开发中，敬请期待！";
                //msg.CreateTime = instanse.CreateTime;
                //msg.FromUserName = instanse.ToUserName;
                //msg.ToUserName = instanse.FromUserName;
                //strResult = pubCore.AutoResponse(msg);
                PubSendMsgText msg = new PubSendMsgText("开发中，敬请期待！", instanse.FromUserName);
                pubCore.SendMsg(msg);
                return "success";
            }
            return strResult;
        }

        public static string DoSubscribe(PubRecEventSubscribe instanse)
        {
            //PubSendMsgText msg = new PubSendMsgText("欢迎您关注\n上海工程技术大学财务处公众号\n我们将秉承\n构建服务型窗口的一贯宗旨\n将便捷的服务带入您的掌上生活\n在这里，您可以\n便捷的查询薪资信息、项目信息\n我们也将陆续开通更多人性化功能\n程财小天使愿随时随地为您服务", instanse.FromUserName);
            PubSendMsgText msg = new PubSendMsgText("回复：\n 1、输入关键字“个人所得税”即可出现个税税率计算表等内容；\n2、输入关键字“学校账号”即可出现学校基本开户行信息；\n3、输入关键字“纳税人识别号”即可出现学校纳税人识别号；\n4、输入关键字“教师报销”即可出现教师报销业务常见问题；\n5、输入关键字“学生报销”即可出现学生报销业务常见问题；\n6、输入“银行服务”即可查阅具体各银行上门服务时间；\n7、输入“差旅费”即可查询差旅费报销规定；\n8、再次感谢关注上海工程技术大学财务处官方微信平台，如有任何疑问或者建议请直接联系我们财务处。", instanse.FromUserName);
            pubCore.SendMsg(msg);
            return "success";
        }

        public static string DoMsgText(PubRecMsgText instanse)
        {
            //PubResMsgText msg = new PubResMsgText();
            string strResult = string.Empty;
            if ("createmenu".Equals(instanse.Content.ToLower()))
            {
                CreateMenu();
            }
            else if ("materialcondition".Equals(instanse.Content.ToLower()))
            {
                MaterialCondition condition = new MaterialCondition(MaterialTypeEnum.news,0,20);
                pubCore.batchget_material(condition);
            }
            else if ("个人所得税".Equals(instanse.Content))
            {
                PubSendMsgMpnews mpnews = new PubSendMsgMpnews();
                mpnews.touser = instanse.FromUserName;
                mpnews.mpnews.media_id = "SitB_ly1YP7cYE4v-8ZkxWXl9PSzK3OMUIc8_fERI8c";
                pubCore.SendMsg(mpnews);
                strResult = "success";
            }
            else if ("学校账户".Equals(instanse.Content)|| "账号".Equals(instanse.Content)|| "学校账号".Equals(instanse.Content)|| "银行账号".Equals(instanse.Content)|| "学校帐户".Equals(instanse.Content) || "帐号".Equals(instanse.Content) || "学校帐号".Equals(instanse.Content) || "银行帐号".Equals(instanse.Content))
            {
                //msg.Content = "上海工程技术大学\n31982603001717943\n上海银行松江支行";
                //msg.CreateTime = instanse.CreateTime;
                //msg.FromUserName = instanse.ToUserName;
                //msg.ToUserName = instanse.FromUserName;
                //strResult = pubCore.AutoResponse(msg);
                PubSendMsgText msg = new PubSendMsgText("上海工程技术大学\n31982603001717943\n上海银行松江支行", instanse.FromUserName);
                pubCore.SendMsg(msg);
                return "success";
            }
            else if ("纳税人识别号".Equals(instanse.Content) || "税号".Equals(instanse.Content) || "学校税号".Equals(instanse.Content) || "开票信息".Equals(instanse.Content))
            {
                //msg.Content = "名称：上海工程技术大学\n纳税人识别号：310105425022547\n地址 ：上海市松江区龙腾路333号\n开户行及账号：上海银行松江支行：31982603001717943";
                //msg.CreateTime = instanse.CreateTime;
                //msg.FromUserName = instanse.ToUserName;
                //msg.ToUserName = instanse.FromUserName;
                //strResult = pubCore.AutoResponse(msg);
                PubSendMsgText msg = new PubSendMsgText("名称：上海工程技术大学\n纳税人识别号：310105425022547\n地址 ：上海市松江区龙腾路333号\n开户行及账号：上海银行松江支行：31982603001717943", instanse.FromUserName);
                pubCore.SendMsg(msg);
                return "success";
            }
            else if ("报销业务常见问题".Equals(instanse.Content)|| "教师报销".Equals(instanse.Content))
            {
                PubSendMsgMpnews mpnews = new PubSendMsgMpnews();
                mpnews.touser = instanse.FromUserName;
                mpnews.mpnews.media_id = "SitB_ly1YP7cYE4v-8ZkxeR3-nQaOnRQUBA8gsjsy_8";
                pubCore.SendMsg(mpnews);
                strResult = "success";
            }
            else if ("大学生创新项目".Equals(instanse.Content) || "学生报销".Equals(instanse.Content) || "学生".Equals(instanse.Content))
            {
                PubSendMsgMpnews mpnews = new PubSendMsgMpnews();
                mpnews.touser = instanse.FromUserName;
                mpnews.mpnews.media_id = "SitB_ly1YP7cYE4v-8ZkxfcscwdzfvQWZxxukBEV2uA";
                pubCore.SendMsg(mpnews);
                strResult = "success";
            }
            else if ("银行上门服务时间".Equals(instanse.Content) || "上门服务".Equals(instanse.Content) || "银行服务".Equals(instanse.Content))
            {
                PubSendMsgMpnews mpnews = new PubSendMsgMpnews();
                mpnews.touser = instanse.FromUserName;
                mpnews.mpnews.media_id = "SitB_ly1YP7cYE4v-8ZkxUnsUDjLYgmEVUmEVcyibQQ";
                pubCore.SendMsg(mpnews);
                strResult= "success";
            }
            else if ("差旅费".Equals(instanse.Content))
            {
                PubSendMsgMpnews mpnews = new PubSendMsgMpnews();
                mpnews.touser = instanse.FromUserName;
                mpnews.mpnews.media_id = "SitB_ly1YP7cYE4v-8Zkxbg2WpgT2JOhD8DY6_KICeQ";
                pubCore.SendMsg(mpnews);
                strResult = "success";
            }
            else if ("帮助".Equals(instanse.Content) )
            {
                //msg.Content = "回复：\n 1、输入关键字“个人所得税”即可出现个税税率计算表等内容；\n2、输入关键字“学校账号”即可出现学校基本开户行信息；\n3、输入关键字“纳税人识别号”即可出现学校纳税人识别号；\n4、输入关键字“教师报销”即可出现教师报销业务常见问题；\n5、输入关键字“学生报销”即可出现学生报销业务常见问题；\n6、输入“银行服务”即可查阅具体各银行上门服务时间；\n7、输入“差旅费”即可查询差旅费报销规定；\n8、再次感谢关注上海工程技术大学财务处官方微信平台，如有任何疑问或者建议请直接联系我们财务处。";
                //msg.CreateTime = instanse.CreateTime;
                //msg.FromUserName = instanse.ToUserName;
                //msg.ToUserName = instanse.FromUserName;
                //strResult = pubCore.AutoResponse(msg);
                PubSendMsgText msg = new PubSendMsgText("回复：\n 1、输入关键字“个人所得税”即可出现个税税率计算表等内容；\n2、输入关键字“学校账号”即可出现学校基本开户行信息；\n3、输入关键字“纳税人识别号”即可出现学校纳税人识别号；\n4、输入关键字“教师报销”即可出现教师报销业务常见问题；\n5、输入关键字“学生报销”即可出现学生报销业务常见问题；\n6、输入“银行服务”即可查阅具体各银行上门服务时间；\n7、输入“差旅费”即可查询差旅费报销规定；\n8、再次感谢关注上海工程技术大学财务处官方微信平台，如有任何疑问或者建议请直接联系我们财务处。", instanse.FromUserName);
                pubCore.SendMsg(msg);
                return "success";
            }
            else
            {
                //msg.Content = "回复：\n 1、输入关键字“个人所得税”即可出现个税税率计算表等内容；\n2、输入关键字“学校账号”即可出现学校基本开户行信息；\n3、输入关键字“纳税人识别号”即可出现学校纳税人识别号；\n4、输入关键字“教师报销”即可出现教师报销业务常见问题；\n5、输入关键字“学生报销”即可出现学生报销业务常见问题；\n6、输入“银行服务”即可查阅具体各银行上门服务时间；\n7、输入“差旅费”即可查询差旅费报销规定；\n8、再次感谢关注上海工程技术大学财务处官方微信平台，如有任何疑问或者建议请直接联系我们财务处。";
                //msg.CreateTime = instanse.CreateTime;
                //msg.FromUserName = instanse.ToUserName;
                //msg.ToUserName = instanse.FromUserName;
                //strResult = pubCore.AutoResponse(msg);
                PubSendMsgText msg = new PubSendMsgText("回复：\n 1、输入关键字“个人所得税”即可出现个税税率计算表等内容；\n2、输入关键字“学校账号”即可出现学校基本开户行信息；\n3、输入关键字“纳税人识别号”即可出现学校纳税人识别号；\n4、输入关键字“教师报销”即可出现教师报销业务常见问题；\n5、输入关键字“学生报销”即可出现学生报销业务常见问题；\n6、输入“银行服务”即可查阅具体各银行上门服务时间；\n7、输入“差旅费”即可查询差旅费报销规定；\n8、再次感谢关注上海工程技术大学财务处官方微信平台，如有任何疑问或者建议请直接联系我们财务处。", instanse.FromUserName);
                pubCore.SendMsg(msg);
                return "success";

            }
            //log.Info("DoMsgText");        
            return strResult;
        }
 
        public static void CreateMenu()
        {
            RootMenu rootmenu = new RootMenu();
            ChildMenu menu1 = new ChildMenu("程财信息");
            ChildMenu menu2 = new ChildMenu("程财服务");
            ChildMenu menu3 = new ChildMenu("程财大厅");

            ChildMenu menu11 = new ChildMenu("项目查询", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/Login/Index?state=SUES!fund");
            ChildMenu menu12 = new ChildMenu("薪资查询", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:80/Login/Index?state=SUES!salary");
            ChildMenu menu13 = new ChildMenu("来款查询", ChildMenu.MenuTypeEnum.click, "13");
            ChildMenu menu14 = new ChildMenu("报销查询", ChildMenu.MenuTypeEnum.click, "14");
            ChildMenu menu15 = new ChildMenu("学费查询", ChildMenu.MenuTypeEnum.view, "http://cwpt.sues.edu.cn:8080/SFP_slogin");

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