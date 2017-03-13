using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 点击菜单拉取消息的事件推送类
    /// </summary>
    public class CorpRecEventClick : CorpRecEventBase
    {

        /// <summary>
        /// 点击菜单拉取消息的事件
        /// </summary>
        public static event WechatEventHandler<CorpRecEventClick> OnEventClick;        //声明事件

        public CorpRecEventClick(string sMsg)
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

            }
            catch (Exception e)
            {
                log.Error("CorpRecEventClick", e);
            }
        }

        public override string DoProcess()
        {
            string strResult = string.Empty;
            if (OnEventClick != null)
            { //如果有对象注册 
                strResult=OnEventClick(this);  //调用所有注册对象的方法
            }
            return strResult;
        }

        /// <summary>
        /// CLICK:事件KEY值，与自定义菜单接口中KEY值对应
        /// </summary>
        public string EventKey { get; private set; }
    }
}
