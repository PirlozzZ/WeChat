using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    public abstract class CorpRecAbstract
    {
        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string ToUserName { get; protected set; }

        /// <summary>
        /// 发送方帐号（一个OpenID）
        /// </summary>
        public string FromUserName { get; protected set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public string CreateTime { get; protected set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; protected set; }

        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public string AgentID { get; protected set; }

        public delegate string WechatEventHandler<in T>(T instanse);   //声明委托

        public abstract string DoProcess();

        public abstract string ToXML();
    }
}
