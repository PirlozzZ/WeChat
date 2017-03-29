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
    public class CorpResMsgVideo : CorpResMsgBase
    {
        public CorpResMsgVideo()
        {
            this.MsgType = "video";
        }

        public CorpResMsgVideo(CorpRecMsgBase instanse)
        {
            this.MsgType = "video";
            this.CreateTime = instanse.CreateTime;
            this.FromUserName = instanse.ToUserName;
            this.ToUserName = instanse.FromUserName;
        }

        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string MediaId { get; set; }

        /// <summary>
        /// 视频消息的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视频消息的描述
        /// </summary>
        public string Description { get; set; }
    }
}
