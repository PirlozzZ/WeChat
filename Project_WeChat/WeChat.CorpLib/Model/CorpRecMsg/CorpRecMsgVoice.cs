using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    public class CorpRecMsgVoice : CorpRecMsgBase
    {

        public CorpRecMsgVoice(string sMsg)
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
                this.AgentID = root["AgentID"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("CorpRecMsgVoice", e);
            }
        }

        public static event WechatEventHandler<CorpRecMsgVoice> OnMsgVoice;        //声明事件
        public override string DoProcess()
        {

            string strResult = string.Empty;
            if (OnMsgVoice != null)
            { //如果有对象注册 
                strResult=OnMsgVoice(this);  //调用所有注册对象的方法
            }
            return strResult;
        }

        /// <summary>
        /// 语音格式，如amr，speex等
        /// </summary>
        public string Format { get; private set; }

        /// <summary>
        /// 语音消息媒体id，可以调用多媒体文件下载接口拉取该媒体
        /// </summary>
        public string MediaId { get; private set; }
         
    }
}
