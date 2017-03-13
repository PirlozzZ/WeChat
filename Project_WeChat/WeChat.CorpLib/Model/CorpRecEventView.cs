using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{

    /// <summary>
    /// 点击菜单跳转链接的事件推送类
    /// </summary>
    public class CorpRecEventView : CorpRecEventBase
    {
        /// <summary>
        /// 点击菜单跳转链接的事件
        /// </summary>
        public static event WechatEventHandler<CorpRecEventView> OnEventView;        //声明事件

        public CorpRecEventView(string sMsg)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sMsg);
                XmlNode root = doc.FirstChild;
                this.ToUserName = root["ToUserName"].InnerText;
                this.FromUserName = root["FromUserName"].InnerText;
                this.CreateTime = root["CreateTime"].InnerText;
                this.MsgType = root["MsgType"].InnerText;
                this.Event = root["Event"].InnerText;
                this.EventKey = root["EventKey"].InnerText;
                this.AgentID = root["AgentID"].InnerText;
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventView", e);
            }
        }

        public override string DoProcess()
        {
            string strResult = string.Empty;
            if (OnEventView != null)
            { //如果有对象注册 
                strResult=OnEventView(this);  //调用所有注册对象的方法
            }
            return strResult;
        }

        /// <summary> 
        /// VIEW:事件KEY值，设置的跳转URL
        /// </summary>
        public string EventKey { get; private set; }
    }
}
