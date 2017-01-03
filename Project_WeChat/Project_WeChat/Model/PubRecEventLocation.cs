using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Project_WeChat.Model
{
    /// <summary>
    /// 事件类型，LOCATION
    /// </summary>
    public class PubRecEventLocation: PubRecEventBase
    {
        public PubRecEventLocation(string sMsg)
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
                this.Latitude = root["Latitude"].InnerText;
                this.Longitude = root["Longitude"].InnerText;
                this.Precision = root["Precision"].InnerText;
            }
            catch (Exception e)
            {
                log.Error("PubRecEventLocation", e);
            }
        }



        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Latitude { get; private set; }

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Longitude { get; private set; }

        /// <summary>
        /// 地理位置精度
        /// </summary>
        public string Precision { get; private set; }
    }
}