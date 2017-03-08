using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.PubLib.Model
{

    /// <summary>
    /// 回复语音消息
    /// </summary>
    public class PubResMsgVoice : PubResMsgBase
    {
        public PubResMsgVoice()
        {
            this.MsgType = "voice";
        }

        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }
    }
}
