using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
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

        public static event WechatEventHandler<PubRecMsgLink> OnMsgLink;        //声明事件
        public override string DoProcess()
        {
            string strResult = string.Empty;
            if (OnMsgLink != null)
            { //如果有对象注册 
                strResult=OnMsgLink(this);  //调用所有注册对象的方法
            }
            return strResult;
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