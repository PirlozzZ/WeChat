using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
{
    /// <summary>
    /// 事件类型，subscribe(订阅)、unsubscribe(取消订阅)
    /// </summary>
    public class PubRecEventSubscribe:PubRecEventBase
    {
        public static event WechatEventHandler<PubRecEventSubscribe> OnEventSubscribe;        //声明事件

        public PubRecEventSubscribe(string sMsg)
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
                if (root["EventKey"] != null)
                {
                    this.EventKey = root["EventKey"].InnerText;
                    this.Ticket = root["Ticket"].InnerText;
                }

            }
            catch (Exception e)
            {
                log.Error("PubRecEventSubscribe", e);
            }
        }
         

        /// <summary>
        /// 1. 用户未关注时，进行关注后的事件推送：事件KEY值，qrscene_为前缀，后面为二维码的参数值
        /// 2. 用户已关注时的事件推送：事件KEY值，是一个32位无符号整数，即创建二维码时的二维码scene_id
        /// </summary>
        public string EventKey { get; private set; }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; private set; }

        public override void DoProcess()
        {
            if (OnEventSubscribe != null)
            { //如果有对象注册 
                OnEventSubscribe(this);  //调用所有注册对象的方法
            }
        }
    }
}