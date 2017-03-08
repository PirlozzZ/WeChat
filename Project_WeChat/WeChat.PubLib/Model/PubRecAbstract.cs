using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public abstract class PubRecAbstract
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

        public delegate void WechatEventHandler<in T>(T instanse);   //声明委托

        public abstract void DoProcess();

        //public abstract string ToXML();
    }
}