using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 回复文本消息
    /// </summary>
    public class CorpResMsgText:CorpResMsgBase
    {
        public CorpResMsgText()
        {
            this.MsgType = "text";
        }

        /// <summary>
        /// 回复的消息内容（换行：在content中能够换行，微信客户端就支持换行显示）
        /// </summary>
        public string Content { get; set; }
    }
}
