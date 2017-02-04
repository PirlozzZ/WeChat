using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace Project_WeChat.Model
{
    public class PubRecMsgImage:PubRecMsgBase
    {
 
        public PubRecMsgImage(string sMsg)
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
                this.PicUrl = root["PicUrl"].InnerText;
                this.MediaId = root["MediaId"].InnerText;
                this.MsgId = root["MsgId"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("PubRecMsgImage", e);
            }
        }

        public static event WechatEventHandler<PubRecMsgImage> OnMsgImage;        //声明事件
        public override void DoProcess()
        {
            if (OnMsgImage != null)
            { //如果有对象注册 
                OnMsgImage(this);  //调用所有注册对象的方法
            }
        }

        /// <summary>
        /// 图片链接（由系统生成）
        /// </summary>
        public string PicUrl { get; private set; }

        /// <summary>
        /// 图片消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; private set; }
    }
}