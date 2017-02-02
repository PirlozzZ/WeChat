using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Project_WeChat.Model
{
    /// <summary>
    /// 事件类型，CLICK、VIEW
    /// </summary>
    public class PubRecEventMenu : PubRecEventBase
    {
        public static event EventHandler<PubRecEventMenu> MenuEventHandler;        //声明事件

        public PubRecEventMenu(string sMsg)
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
            }
            catch (Exception e)
            {
                log.Error("PubRecEventClick", e);
            }
        }

        public override void DoProcess()
        {
            if (MenuEventHandler != null)
            { //如果有对象注册 
                MenuEventHandler(this);  //调用所有注册对象的方法
            } 
        }

        /// <summary>
        /// CLICK:事件KEY值，与自定义菜单接口中KEY值对应
        /// VIEW:事件KEY值，设置的跳转URL
        /// </summary>
        public string EventKey { get; private set; }
    }
}