using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{

    /// <summary>
    /// 回复语音消息
    /// </summary>
    public class CorpResMsgVoice : CorpResMsgBase
    {
        public CorpResMsgVoice()
        {
            this.MsgType = "voice";
        }

        public CorpResMsgVoice(CorpRecMsgBase instanse)
        {
            this.MsgType = "voice";
            this.CreateTime = instanse.CreateTime;
            this.FromUserName = instanse.ToUserName;
            this.ToUserName = instanse.FromUserName;
        }

        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }
    }
}
