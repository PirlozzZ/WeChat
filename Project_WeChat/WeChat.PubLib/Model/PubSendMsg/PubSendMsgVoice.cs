using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgVoice:PubSendMsgBase
    {
        public PubSendMsgVoice()
        {
            this.voice = new VoiceMain();
            this.msgtype = "voice";
        }

        public VoiceMain voice { get; set; }
         
    }

    public class VoiceMain
    {
        /// <summary>
        /// 语音文件id，可以调用上传临时素材或者永久素材接口获取
        /// </summary>
        public string media_id { get; set; }
    }
}
