using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public  class PubRecEventBase:PubRecMsgBase
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public new string MsgId { get; private set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string Event { get; protected set; }
        
    }
}