using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using WeChat.CorpLib.Core;
using WeChat.CorpLib.Model;

namespace WeChat.WebApp
{
    /// <summary>
    /// CorpWeChatSheczzx 的摘要说明
    /// </summary>
    public class CorpWeChatSheczzx : IHttpHandler
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        static CorpCore corpCoreSheczzx;
        static string logoutURL = ConfigurationManager.AppSettings["SheczzxLogoutURL"]; 

        static CorpWeChatSheczzx()
        {
            corpCoreSheczzx = new CorpCore("Sheczzx");
            CorpRecEventClick.OnEventClick += DoClick; 
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
            string pMsgSignature = HttpContext.Current.Request.QueryString["msg_signature"];
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



                log.Debug("CorpWeChatSheczzx ProcessRequest Get:" + postStr);
                if (!string.IsNullOrEmpty(postStr))
                {
                    sResult = corpCoreSheczzx.ProcessMsg(postStr, pMsgSignature, pTimeStamp, pNonce);
                    log.Debug("CorpWeChatSheczzx ProcessRequest sResult:" + sResult);
                }
                HttpContext.Current.Response.Write(sResult);
                HttpContext.Current.ApplicationInstance.CompleteRequest();

            }

            else
            {
                string pEchoStr = HttpContext.Current.Request.QueryString["echostr"];
                try
                { 
                    pEchoStr = corpCoreSheczzx.CorpAuth(pTimeStamp, pNonce, pEchoStr, pMsgSignature);
                    log.Debug("CorpWeChatSheczzx ProcessRequest after pEchoStr:" + pEchoStr);
                    HttpContext.Current.Response.Write(pEchoStr);
                    HttpContext.Current.ApplicationInstance.CompleteRequest();

                }
                catch (Exception e)
                {
                    log.Error("ProcessRequest Get:", e);//写入一条新log
                }
            }
        }

        #region 业务逻辑
        public static string DoClick(CorpRecEventClick instanse)
        {
            string strResult = string.Empty;
            if ("1".Equals(instanse.EventKey))
            {


                //CorpSendMsgText msg = new CorpSendMsgText("教委办公室：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=委办  \n发展规划处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=规划 \n人事处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=人事 \n财务处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=财务 \n基教处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=基教 \n职教处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=职教 \n高教处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=高教 \n学生处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=学生 \n科技处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=科技 \n体卫艺科处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=体卫艺科 \n国际交流处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=国交 \n后勤保卫处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=后保 \n政策法规处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=法规 \n语言文字处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=语言文字 \n审计处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=审计 \n德育处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=德育 \n督导室：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=督导 \n终身教育处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=终教 \n民办教育处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=民教 \n青保处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=青保 \n", instanse.FromUserName, agentid);

                CorpSendMsgText msg = new CorpSendMsgText("教委办公室: http://www.shmecsfa.shec.edu.cn/Login/Login/wb%e5%a7%94%e5%8a%9e \n发展规划处: http://www.shmecsfa.shec.edu.cn/Login/Login/gh%e8%a7%84%e5%88%92 \n人事处: http://www.shmecsfa.shec.edu.cn/Login/Login/rs%e4%ba%ba%e4%ba%8b \n财务处: http://www.shmecsfa.shec.edu.cn/Login/Login/cw%e8%b4%a2%e5%8a%a1 \n基教处: http://www.shmecsfa.shec.edu.cn/Login/Login/jj%e5%9f%ba%e6%95%99 \n职教处: http://www.shmecsfa.shec.edu.cn/Login/Login/zj%e8%81%8c%e6%95%99 \n高教处: http://www.shmecsfa.shec.edu.cn/Login/Login/gj%e9%ab%98%e6%95%99 \n学生处: http://www.shmecsfa.shec.edu.cn/Login/Login/xs%e5%ad%a6%e7%94%9f \n科技处: http://www.shmecsfa.shec.edu.cn/Login/Login/kj%e7%a7%91%e6%8a%80 \n体卫艺科处: http://www.shmecsfa.shec.edu.cn/Login/Login/tw%e4%bd%93%e5%8d%ab%e8%89%ba%e7%a7%91 \n国际交流处: http://www.shmecsfa.shec.edu.cn/Login/Login/go%e5%9b%bd%e4%ba%a4 \n后勤保卫处: http://www.shmecsfa.shec.edu.cn/Login/Login/hb%e5%90%8e%e4%bf%9d \n政策法规处: http://www.shmecsfa.shec.edu.cn/Login/Login/fg%e6%b3%95%e8%a7%84 \n语言文字处: http://www.shmecsfa.shec.edu.cn/Login/Login/yy%e8%af%ad%e8%a8%80%e6%96%87%e5%ad%97 \n审计处: http://www.shmecsfa.shec.edu.cn/Login/Login/sj%e5%ae%a1%e8%ae%a1 \n德育处: http://www.shmecsfa.shec.edu.cn/Login/Login/dy%e5%be%b7%e8%82%b2 \n督导室: http://www.shmecsfa.shec.edu.cn/Login/Login/dd%e7%9d%a3%e5%af%bc \n终身教育处: http://www.shmecsfa.shec.edu.cn/Login/Login/zs%e7%bb%88%e6%95%99 \n民办教育处: http://www.shmecsfa.shec.edu.cn/Login/Login/mj%e6%b0%91%e6%95%99 \n青保处: http://www.shmecsfa.shec.edu.cn/Login/Login/qb%e9%9d%92%e4%bf%9d \n", instanse.FromUserName);
                corpCoreSheczzx.SendMsg(msg); 
            }
            //else if ("21".Equals(instanse.EventKey))
            //{
            //    NewsMain nm1 = new NewsMain();
            //    nm1.title = "申报单位操作说明";
            //    //nm1.picurl = "http://wx.seaskysh.com.cn/SeaskyWechatCent/material/封面.png";
            //    nm1.picurl = "https://qy.weixin.qq.com/cgi-bin/getmediadata?token=1182936902&lang=zh_CN&type=image&name=%E5%B0%81%E9%9D%A2.png&media_id=2UKhHwnp8zOZgyWtKNf-8uVClA95JZY1UQZ8BvymIj0qzrK3GhdDycqYbThehIJNj";
            //    nm1.url = "http://h5.wps.cn/p/226e667e.html";
            //    CorpSendMsgNews msg = new CorpSendMsgNews();
            //    msg.agentid = agentid;
            //    msg.touser = instanse.FromUserName; 
            //    msg.news.articles.Add(nm1); 
            //    corpCore.SendMsg(msg);
            //}
            else if("22".Equals(instanse.EventKey))
            {
                 
                CorpSendMsgText msg = new CorpSendMsgText("开发中，敬请期待！", instanse.ToUserName);
                 
                corpCoreSheczzx.SendMsg(msg);
            }
            else
            {
                CorpSendMsgText msg = new CorpSendMsgText("开发中，敬请期待！", instanse.ToUserName);
              
                corpCoreSheczzx.SendMsg(msg);
            }
            return strResult;
        }

        public static string DoSubscribe(CorpRecEventSubscribe instanse)
        {
            //CorpSendMsgText msg = new CorpSendMsgText("教委办公室：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=委办  \n发展规划处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=规划 \n人事处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=人事 \n财务处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=财务 \n基教处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=基教 \n职教处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=职教 \n高教处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=高教 \n学生处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=学生 \n科技处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=科技 \n体卫艺科处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=体卫艺科 \n国际交流处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=国交 \n后勤保卫处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=后保 \n政策法规处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=法规 \n语言文字处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=语言文字 \n审计处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=审计 \n德育处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=德育 \n督导室：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=督导 \n终身教育处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=终教 \n民办教育处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=民教 \n青保处：http://www.shmecsfa.shec.edu.cn/Login/Login?dcode=青保！ \n", instanse.FromUserName, agentid);
            CorpSendMsgText msg = new CorpSendMsgText("教委办公室: http://www.shmecsfa.shec.edu.cn/Login/Login/wb%e5%a7%94%e5%8a%9e \n发展规划处: http://www.shmecsfa.shec.edu.cn/Login/Login/gh%e8%a7%84%e5%88%92 \n人事处: http://www.shmecsfa.shec.edu.cn/Login/Login/rs%e4%ba%ba%e4%ba%8b \n财务处: http://www.shmecsfa.shec.edu.cn/Login/Login/cw%e8%b4%a2%e5%8a%a1 \n基教处: http://www.shmecsfa.shec.edu.cn/Login/Login/jj%e5%9f%ba%e6%95%99 \n职教处: http://www.shmecsfa.shec.edu.cn/Login/Login/zj%e8%81%8c%e6%95%99 \n高教处: http://www.shmecsfa.shec.edu.cn/Login/Login/gj%e9%ab%98%e6%95%99 \n学生处: http://www.shmecsfa.shec.edu.cn/Login/Login/xs%e5%ad%a6%e7%94%9f \n科技处: http://www.shmecsfa.shec.edu.cn/Login/Login/kj%e7%a7%91%e6%8a%80 \n体卫艺科处: http://www.shmecsfa.shec.edu.cn/Login/Login/tw%e4%bd%93%e5%8d%ab%e8%89%ba%e7%a7%91 \n国际交流处: http://www.shmecsfa.shec.edu.cn/Login/Login/go%e5%9b%bd%e4%ba%a4 \n后勤保卫处: http://www.shmecsfa.shec.edu.cn/Login/Login/hb%e5%90%8e%e4%bf%9d \n政策法规处: http://www.shmecsfa.shec.edu.cn/Login/Login/fg%e6%b3%95%e8%a7%84 \n语言文字处: http://www.shmecsfa.shec.edu.cn/Login/Login/yy%e8%af%ad%e8%a8%80%e6%96%87%e5%ad%97 \n审计处: http://www.shmecsfa.shec.edu.cn/Login/Login/sj%e5%ae%a1%e8%ae%a1 \n德育处: http://www.shmecsfa.shec.edu.cn/Login/Login/dy%e5%be%b7%e8%82%b2 \n督导室: http://www.shmecsfa.shec.edu.cn/Login/Login/dd%e7%9d%a3%e5%af%bc \n终身教育处: http://www.shmecsfa.shec.edu.cn/Login/Login/zs%e7%bb%88%e6%95%99 \n民办教育处: http://www.shmecsfa.shec.edu.cn/Login/Login/mj%e6%b0%91%e6%95%99 \n青保处: http://www.shmecsfa.shec.edu.cn/Login/Login/qb%e9%9d%92%e4%bf%9d \n", instanse.FromUserName);
            corpCoreSheczzx.SendMsg(msg);
            return "success";
        }


        #endregion
    }
}