using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
{
    public class PubRecMsgText: PubRecMsgBase
    {
         
        public PubRecMsgText(string sMsg)
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
                this.Content = root["Content"].InnerText;
                this.MsgId = root["MsgId"].InnerText;

            }
            catch (Exception e)
            { 
                log.Error("PubRecMsgText", e);
            }
        }

        public static event WechatEventHandler<PubRecMsgText> OnMsgText;        //声明事件
        public override void DoProcess()
        {
            if (OnMsgText != null)
            { //如果有对象注册 
                OnMsgText(this);  //调用所有注册对象的方法
            }
        }

        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; private set; }
    }
}