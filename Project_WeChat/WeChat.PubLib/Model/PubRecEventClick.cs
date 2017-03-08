using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
{
    /// <summary>
    /// 自定义菜单点击事件类
    /// </summary>
    public class PubRecEventClick : PubRecEventBase
    {
        /// <summary>
        /// 自定义菜单点击事件
        /// </summary>
        public static event WechatEventHandler<PubRecEventClick> OnEventClick;        //声明事件

        public PubRecEventClick(string sMsg)
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
            if (OnEventClick != null)
            { //如果有对象注册 
                OnEventClick(this);  //调用所有注册对象的方法
            } 
        }

        /// <summary>
        /// CLICK:事件KEY值，与自定义菜单接口中KEY值对应
        /// </summary>
        public string EventKey { get; private set; }
    }
}