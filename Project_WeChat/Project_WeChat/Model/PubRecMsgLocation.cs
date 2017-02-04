using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Project_WeChat.Model
{
    public class PubRecMsgLocation:PubRecMsgBase
    {
        public PubRecMsgLocation(string sMsg)
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
                this.Location_X = root["Location_X"].InnerText;
                this.Location_Y = root["Location_Y"].InnerText;
                this.Label = root["Label"].InnerText;
                this.Scale = root["Scale"].InnerText;
                this.MsgId = root["MsgId"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("PubRecMsgLocation", e);
            }
        }

        public static event WechatEventHandler<PubRecMsgLocation> OnMsgLocation;        //声明事件
        public override void DoProcess()
        {
            if (OnMsgLocation != null)
            { //如果有对象注册 
                OnMsgLocation(this);  //调用所有注册对象的方法
            }
        }

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public string Location_X { get; private set; }
        /// <summary>
        /// 地理位置经度
        /// </summary>
        public string Location_Y { get; private set; }
        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public string Scale { get; private set; }
        /// <summary>
        /// 地理位置信息
        /// </summary>
        public string Label { get; private set; }
    }
}