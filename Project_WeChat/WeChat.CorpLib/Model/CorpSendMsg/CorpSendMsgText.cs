using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.CorpLib.Model
{
    public class CorpSendMsgText :CorpSendMsgBase
    {
        public TextMain text { get; set; }

        public CorpSendMsgText()
        {
            this.text = new TextMain();
            this.msgtype = "text";
        }

        /// <summary>
        /// 带参构造
        /// </summary>
        /// <param name="content">消息内容</param>
        /// <param name="touser">发送对象，成员ID列表（消息接收者，多个接收者用‘|’分隔，最多支持1000个）。特殊情况：指定为@all，则向关注该企业应用的全部成员发送</param>
        /// <param name="agentid">应用ID</param>
        public CorpSendMsgText(string content, string touser, string agentid)
        {

            this.text = new TextMain();
            this.msgtype = "text";
            this.text.content = content;
            this.touser = touser;
            this.agentid = agentid;
        }
    }

    public class TextMain
    {
        /// <summary>
        /// 消息内容，最长不超过2048个字节，注意：主页型应用推送的文本消息在微信端最多只显示20个字（包含中英文）
        /// </summary>
        public string content { get; set; }
    }
}
