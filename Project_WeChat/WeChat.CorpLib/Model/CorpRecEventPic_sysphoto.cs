using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    public class CorpRecEventPic_sysphoto : CorpRecEventBase
    {
        public static event WechatEventHandler<CorpRecEventPic_sysphoto> OnEventPic_sysphoto;        //声明事件

        public CorpRecEventPic_sysphoto(string sMsg)
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
                this.EventKey = root["EventKey"].InnerText;
                this.AgentID = root["AgentID"].InnerText;
                this.SendPicsInfo = root["SendPicsInfo"].InnerText;
                this.Count = root["Count"].InnerText;
                this.PicList = root["PicList"].InnerText;
                this.PicMd5Sum = root["PicMd5Sum"].InnerText;
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventPic_sysphoto", e);
            }
        }

        public override void DoProcess()
        {
            if (OnEventPic_sysphoto != null)
            { //如果有对象注册 
                OnEventPic_sysphoto(this);  //调用所有注册对象的方法
            }
        }

        /// <summary> 
        /// 事件KEY值，由开发者在创建菜单时设定L
        /// </summary>
        public string EventKey { get; private set; }

        /// <summary> 
        /// 发送的图片信息
        /// </summary>
        public string SendPicsInfo { get; private set; }

        /// <summary> 
        /// 发送的图片数量
        /// </summary>
        public string Count { get; private set; }

        /// <summary> 
        /// 图片列表
        /// </summary>
        public string PicList { get; private set; }

        /// <summary> 
        /// 图片的MD5值，开发者若需要，可用于验证接收到图片
        /// </summary>
        public string PicMd5Sum { get; private set; }
    }
}
