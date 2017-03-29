using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 回复图片消息
    /// </summary>
    public class CorpResMsgImage : CorpResMsgBase
    {
        public CorpResMsgImage()
        {
            this.MsgType = "image";
        }

        public CorpResMsgImage(CorpRecMsgBase instanse)
        {
            this.MsgType = "image";
            this.CreateTime = instanse.CreateTime;
            this.FromUserName = instanse.ToUserName;
            this.ToUserName = instanse.FromUserName;
        }

        /// <summary>
        /// 通过素材管理中的接口上传多媒体文件，得到的id。
        /// </summary>
        public string MediaId { get; set; }
    }
}
