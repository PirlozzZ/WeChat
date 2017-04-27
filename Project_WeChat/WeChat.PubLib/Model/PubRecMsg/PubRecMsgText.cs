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
        public override string DoProcess()
        {
            string strResult = string.Empty;
            if (OnMsgText != null)
            { //如果有对象注册 
                //log.Debug("PubRecMsgText OnMsgText" + OnMsgText);
                strResult =OnMsgText(this);  //调用所有注册对象的方法
                
            }
            return strResult;
        }

        /// <summary>
        /// 文本消息内容
        /// </summary>
        public string Content { get; private set; }

        public static bool isEventNull()
        {
            bool sign = true;
            if (OnMsgText != null)
            {
                sign = false;
            }
            return sign;
        }
    }
}