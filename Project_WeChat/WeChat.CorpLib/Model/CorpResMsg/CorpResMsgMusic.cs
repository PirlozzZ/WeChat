using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 回复音乐消息
    /// </summary>
    public class CorpResMsgMusic : CorpResMsgBase
    {
        public CorpResMsgMusic()
        {
            this.MsgType = "music";
        }

        public CorpResMsgMusic(CorpRecMsgBase instanse)
        {
            this.MsgType = "music";
            this.CreateTime = instanse.CreateTime;
            this.FromUserName = instanse.ToUserName;
            this.ToUserName = instanse.FromUserName;
        }

        /// <summary>
        /// 视频消息的标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 视频消息的描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 音乐链接
        /// </summary>
        public string MusicURL { get; set; }

        /// <summary>
        /// 高质量音乐链接，WIFI环境优先使用该链接播放音乐
        /// </summary>
        public string HQMusicUrl { get; set; }

        /// <summary>
        /// 缩略图的媒体id，通过素材管理中的接口上传多媒体文件，得到的id
        /// </summary>
        public string ThumbMediaId { get; set; }
    }
}
