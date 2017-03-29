using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.CorpLib.Model
{
    public class CorpSendMsgMpnews : CorpSendMsgBase
    {
        public CorpSendMsgMpnews()
        {
            this.msgtype = "mpnews";
            this.mpnews = new Articles();
        }
        public Articles mpnews { get; set; }

        /// <summary>
        ///图文消息，一个图文消息支持1到8个图文
        /// </summary>
        public class Articles
        {
            public List<MpnewsMain> articles { get; set; }

            public Articles()
            {
                articles = new List<MpnewsMain>();
            }
        }

    }



    public class MpnewsMain
    {
        /// <summary>
        /// 图文消息的标题，不超过128个字节，超过会自动截断
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 图图文消息缩略图的media_id, 可以在上传多媒体文件接口中获得。此处thumb_media_id即上传接口返回的media_id
        /// </summary>
        public string thumb_media_id { get; set; }

        /// <summary>
        /// 图文消息的作者，不超过64个字节
        /// </summary>
        public string author { get; set; }

        /// <summary>
        /// 图文消息点击“阅读原文”之后的页面链接
        /// </summary>
        public string content_source_url { get; set; }

        /// <summary>
        /// 图文消息的内容，支持html标签，不超过666 K个字节
        /// </summary>
        public string content { get; set; }

        /// <summary>
        /// 图文消息的描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string digest { get; set; }

        /// <summary>
        /// 是否显示封面，1为显示，0为不显示
        /// </summary>
        public string show_cover_pic { get; set; }
    }
}
