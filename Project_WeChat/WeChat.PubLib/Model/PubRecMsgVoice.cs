using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;

namespace WeChat.PubLib.Model
{
    public class PubRecMsgVoice:PubRecMsgBase
    {
 
        public PubRecMsgVoice(string sMsg)
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
                this.Format = root["Format"].InnerText;
                this.MediaId = root["MediaId"].InnerText;
                this.MsgId = root["MsgId"].InnerText;
                if (root["Recongnition"] != null)
                {
                    this.Recongnition = root["Recongnition"].InnerText;
                }

            }
            catch (Exception e)
            {
                log.Error("PubRecMsgVoice", e);
            }
        }

        public static event WechatEventHandler<PubRecMsgVoice> OnMsgVoice;        //声明事件
        public override void DoProcess()
        {
            if (OnMsgVoice != null)
            { //如果有对象注册 
                OnMsgVoice(this);  //调用所有注册对象的方法
            }
        }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取该媒体
        /// </summary>
        public string MediaId { get; private set; }

        /// <summary>
        ///语音识别结果，UTF8编码。开通语音识别后新增
        /// </summary>
        public string Recongnition { get; private set; }
    }
}