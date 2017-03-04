using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    public class CorpRecMsgLink : CorpRecMsgBase
    {
        public CorpRecMsgLink(string sMsg)
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
                this.Title = root["Title"].InnerText;
                this.Description = root["Description"].InnerText;
                this.PicUrl = root["PicUrl"].InnerText;
                this.MsgId = root["MsgId"].InnerText;
                this.AgentID = root["AgentID"].InnerText;

            }
            catch (Exception e)
            {
                log.Error("CorpRecMsgLink", e);
            }
        }

        public static event WechatEventHandler<CorpRecMsgLink> OnMsgLink;        //声明事件
        public override void DoProcess()
        {
            if (OnMsgLink != null)
            { //如果有对象注册 
                OnMsgLink(this);  //调用所有注册对象的方法
            }
        }

        /// <summary>
        /// 消息标题
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// 消息描述
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// 封面缩略图的url
        /// </summary>
        public string PicUrl { get; private set; }
    }
}
