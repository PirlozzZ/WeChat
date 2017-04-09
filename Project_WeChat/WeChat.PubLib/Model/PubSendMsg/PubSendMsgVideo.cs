using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgVideo : PubSendMsgBase
    {
        public VideoMain video { get; set; }

        public PubSendMsgVideo()
        {
            this.video = new VideoMain();
            this.msgtype = "video";
        }
    }
    public class VideoMain
    {
        /// <summary>
        /// 视频媒体文件id，可以调用上传临时素材或者永久素材接口获取
        /// </summary>
        public string media_id { get; set; }

        /// <summary>
        /// 视频消息的标题，不超过128个字节，超过会自动截断
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 视频消息的描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string description { get; set; }
    }
}
