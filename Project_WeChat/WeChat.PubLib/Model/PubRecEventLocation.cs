using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
{
    /// <summary>
    /// 上报地理位置事件类
    /// </summary>
    public class PubRecEventLocation: PubRecEventBase
    {
        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        public static event WechatEventHandler<PubRecEventLocation> OnEventLocation;        //声明事件

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
  
        public override string DoProcess()
        {
            string strResult = string.Empty;
            if (OnEventLocation != null)
            { //如果有对象注册 
                strResult = OnEventLocation(this);  //调用所有注册对象的方法
            }
            return strResult;
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