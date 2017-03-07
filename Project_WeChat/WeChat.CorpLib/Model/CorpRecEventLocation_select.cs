using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 弹出地理位置选择器的事件推送类
    /// </summary>
    public class CorpRecEventLocation_select : CorpRecEventBase
    {

        public CorpRecEventLocation_select(string sMsg)
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
                this.sendLocationInfo = new SendLocationInfo();
                XmlNode nodeSendLocationInfo = root["SendLocationInfo"];
                this.sendLocationInfo.Location_X = nodeSendLocationInfo["Location_X"].InnerText;
                this.sendLocationInfo.Location_Y = nodeSendLocationInfo["Location_Y"].InnerText;
                this.sendLocationInfo.Scale = nodeSendLocationInfo["Scale"].InnerText;
                this.sendLocationInfo.Label = nodeSendLocationInfo["Label"].InnerText;
                this.sendLocationInfo.Poiname = nodeSendLocationInfo["Poiname"].InnerText;
                this.AgentID = root["AgentID"].InnerText;
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventLocation_select", e);
            }
        }
        /// <summary>
        /// 弹出地理位置选择器的事件
        /// </summary>
        public static event WechatEventHandler<CorpRecEventLocation_select> OnEventLocation_select;        //声明事件

        public override void DoProcess()
        {
            if (OnEventLocation_select != null)
            { //如果有对象注册 
                OnEventLocation_select(this);  //调用所有注册对象的方法
            }
        }

        /// <summary> 
        /// 发送的位置信息
        /// </summary>
        public SendLocationInfo sendLocationInfo { get; private set; }

        public class SendLocationInfo
        {
            /// <summary>
            /// X坐标信息
            /// </summary>
            public string Location_X { get; set; }

            /// <summary>
            /// Y坐标信息
            /// </summary>
            public string Location_Y { get; set; }

            /// <summary>
            /// 精度，可理解为精度或者比例尺、越精细的话 scale越高
            /// </summary>
            public string Scale { get; set; }

            /// <summary>
            /// 地理位置的字符串信息
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            /// 朋友圈POI的名字，可能为空
            /// </summary>
            public string Poiname { get; set; }
        }
    }
}
