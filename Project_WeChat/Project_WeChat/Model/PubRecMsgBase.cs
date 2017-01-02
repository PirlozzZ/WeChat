using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_WeChat.Model
{
    public class PubRecMsgBase
    {
        protected log4net.ILog log;
        
        public PubRecMsgBase()
        {
            log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        }

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
        /// 消息id，64位整型
        /// </summary>
        public string MsgId { get; protected set; }


    }
}