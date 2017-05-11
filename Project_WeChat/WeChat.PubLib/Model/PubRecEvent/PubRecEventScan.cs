using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
{
    /// <summary>
    /// 扫描带参数二维码事件类
    /// </summary>
    public class PubRecEventScan : PubRecEventBase
    {

        /// <summary>
        /// 扫描带参数二维码事件
        /// </summary>
        public static event WechatEventHandler<PubRecEventScan> OnEventScan;        //声明事件

        public PubRecEventScan(string sMsg)
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
                this.Ticket = root["Ticket"].InnerText;
                this.EventKey = root["EventKey"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("PubRecEventScan", e);
            }
        }


        /// <summary>
        /// 事件KEY值，是一个32位无符号整数，即创建二维码时的二维码scene_id
        /// </summary>
        public string EventKey { get; private set; }

        /// <summary>
        /// 二维码的ticket，可用来换取二维码图片
        /// </summary>
        public string Ticket { get; private set; }

        public override string DoProcess()
        {          
            string strResult = string.Empty;
            if (OnEventScan != null)
            { //如果有对象注册 
                strResult = OnEventScan(this);  //调用所有注册对象的方法
            }
            return strResult;
        }
    }
}