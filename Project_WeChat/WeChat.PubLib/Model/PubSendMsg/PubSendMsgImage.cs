using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgImage:PubSendMsgBase
    {
        public PubSendMsgImage()
        {
            this.msgtype = "image";
            this.image = new ImageMain();
        }

        public ImageMain image { get; set; }
    }

    public class ImageMain
    {
        /// <summary>
        /// 图片媒体文件id，可以调用上传临时素材或者永久素材接口获取,永久素材media_id必须由发消息的应用创建
        /// </summary>
        public string media_id { get; set; }
    }
}
