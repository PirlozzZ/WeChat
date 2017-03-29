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
    public class CorpResMsgNews : CorpResMsgBase
    {
        public CorpResMsgNews()
        {
            this.MsgType = "news";
            Articles = new ArticlesMain();
        }

        public CorpResMsgNews(CorpRecMsgBase instanse)
        {
            this.MsgType = "news";
            Articles = new ArticlesMain();
            this.CreateTime = instanse.CreateTime;
            this.FromUserName = instanse.ToUserName;
            this.ToUserName = instanse.FromUserName;
        }

        /// <summary>
        /// 多条图文消息信息，默认第一个item为大图,注意，如果图文数超过8，则将会无响应 
        /// </summary>
        public ArticlesMain Articles { get; set; }

        /// <summary>
        /// 图文消息个数，限制为8条以内
        /// </summary>
        public string ArticleCount { get; set; }

        public class ArticlesMain
        {
            public List<NewsMain> item { get; set; }

            public ArticlesMain()
            {
                item = new List<NewsMain>();
            }
        }

        public class NewsMain
        {
            /// <summary>
            /// 图文消息标题
            /// </summary>
            public string title { get; set; }

            /// <summary>
            /// 图文消息描述 
            /// </summary>
            public string description { get; set; }

            /// <summary>
            /// 点击图文消息跳转链接
            /// </summary>
            public string url { get; set; }

            /// <summary>
            /// 图片链接，支持JPG、PNG格式，较好的效果为大图360*200，小图200*200
            /// </summary>
            public string picurl { get; set; }
        }
    }
}
