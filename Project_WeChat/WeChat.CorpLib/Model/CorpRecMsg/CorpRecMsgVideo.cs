using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    public class CorpRecMsgVideo : CorpRecMsgBase
    {
        public CorpRecMsgVideo(string sMsg)
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
                this.ThumbMediaId = root["ThumbMediaId"].InnerText;
                this.MediaId = root["MediaId"].InnerText;
                this.MsgId = root["MsgId"].InnerText;
                this.AgentID = root["AgentID"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("CorpRecMsgVideo", e);
            }
        }

        public static event WechatEventHandler<CorpRecMsgVideo> OnMsgVideo;        //声明事件
        public override string DoProcess()
        {
            string strResult = string.Empty;
            if (OnMsgVideo != null)
            { //如果有对象注册 
                strResult=OnMsgVideo(this);  //调用所有注册对象的方法
            }
            return strResult;
        }

        /// <summary>
        /// 视频消息缩略图的媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string ThumbMediaId { get; private set; }

        /// <summary>
        /// 视频消息媒体id，可以调用多媒体文件下载接口拉取数据。
        /// </summary>
        public string MediaId { get; private set; }
    }
}
