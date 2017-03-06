using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 事件类型，subscribe(订阅)、unsubscribe(取消订阅)
    /// </summary>
    public class CorpRecEventSubscribe : CorpRecEventBase
    {
        public static event WechatEventHandler<CorpRecEventSubscribe> OnEventSubscribe;        //声明事件

        public CorpRecEventSubscribe(string sMsg)
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
                this.AgentID = root["AgentID"].InnerText;
 
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventSubscribe", e);
            }
        }
 
        public override void DoProcess()
        {
            if (OnEventSubscribe != null)
            { //如果有对象注册 
                OnEventSubscribe(this);  //调用所有注册对象的方法
            }
        }
    }
}
