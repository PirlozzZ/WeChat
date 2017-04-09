using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgText :PubSendMsgBase
    {
        public TextMain text { get; set; }

        public PubSendMsgText()
        {
            this.text = new TextMain();
            this.msgtype = "text";
        }

        /// <summary>
        /// 带参构造
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="touser">发送对象，成员ID
        public PubSendMsgText(string content, string touser)
        {

            this.text = new TextMain();
            this.msgtype = "text";
            this.text.content = content;
            this.touser = touser;
        }
    }

    public class TextMain
    {
        /// <summary>
        /// 消息内容
        /// </summary>
        public string content { get; set; }
    }
}
