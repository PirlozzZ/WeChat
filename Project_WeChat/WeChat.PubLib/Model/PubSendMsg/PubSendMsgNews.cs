using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgNews : PubSendMsgBase
    {
        public PubSendMsgNews()
        {
            base.msgtype = "news";
            this.news = new Articles();
        }
        public Articles news { get; set; }

        /// <summary>
        /// 图文消息，一个图文消息支持1到8条图文
        /// </summary>
        public class Articles
        {
            public List<NewsMain> articles { get; set; }

            public Articles()
            {
                articles = new List<NewsMain>();
            }
        }

    }



    public class NewsMain
    {
        /// <summary>
        /// 标题，不超过128个字节，超过会自动截断
        /// </summary>
        public string title { get; set; }

        /// <summary>
        /// 描述，不超过512个字节，超过会自动截断
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 点击后跳转的链接
        /// </summary>
        public string url { get; set; }

        /// <summary>
        /// 图文消息的图片链接，支持JPG、PNG格式，较好的效果为大图640*320，小图80*80。如不填，在客户端不显示图片
        /// </summary>
        public string picurl { get; set; }
    }
}
