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

        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }
    }
}
