using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
     
    public class CorpRecEventPic_photo_or_album : CorpRecEventBase
    {
        public static event WechatEventHandler<CorpRecEventPic_photo_or_album> OnEventPic_photo_or_album;        //声明事件

        public CorpRecEventPic_photo_or_album(string sMsg)
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
                XmlNode nodeSendPicsInfo = root["SendPicsInfo"];
                this.sendPicsInfo = new SendPicsInfo();
                this.sendPicsInfo.Count = nodeSendPicsInfo["Count"].InnerText;
                this.picList = new List<PicList>();
                foreach (XmlNode childnode in nodeSendPicsInfo["PicList"].ChildNodes)
                {
                    PicList piclist = new PicList();
                    piclist.item = new Item();
                    piclist.item.PicMd5Sum = childnode["PicMd5Sum"].InnerText;
                    this.picList.Add(piclist);
                }
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventPic_photo_or_album", e);
            }
        }

        public override void DoProcess()
        {
            if (OnEventPic_photo_or_album != null)
            { //如果有对象注册 
                OnEventPic_photo_or_album(this);  //调用所有注册对象的方法
            }
        }

        /// <summary> 
        /// 事件KEY值，由开发者在创建菜单时设定L
        /// </summary>
        public string EventKey { get; private set; }

        /// <summary> 
        /// 发送的图片信息
        /// </summary>
        public SendPicsInfo sendPicsInfo { get; private set; }



        /// <summary> 
        /// 图片列表
        /// </summary>
        public List<PicList> picList { get; private set; }



        public class SendPicsInfo
        {
            /// <summary> 
            /// 发送的图片数量
            /// </summary>
            public string Count { get; set; }
        }

        public class PicList
        {
            public Item item { get; set; }
        }

        public class Item
        {
            /// <summary> 
            /// 图片的MD5值，开发者若需要，可用于验证接收到图片
            /// </summary>
            public string PicMd5Sum { get; set; }
        }
    }
}
