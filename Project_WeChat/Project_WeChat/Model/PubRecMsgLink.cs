using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Project_WeChat.Model
{
    public class PubRecMsgLink:PubRecMsgBase
    {
        public PubRecMsgLink(string sMsg)
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
                this.Title = root["Title"].InnerText;
                this.Description = root["Description"].InnerText;
                this.Url = root["Url"].InnerText;
                this.MsgId = root["MsgId"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("PubRecMsgLink", e);
            }
        }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 消息链接
        /// </summary>
        public string Url { get; private set; }
    }
}