using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgMpnews : PubSendMsgBase
    {
        public PubSendMsgMpnews()
        {
            this.msgtype = "mpnews";
            this.mpnews = new MpnewsMain();
        }
        public MpnewsMain mpnews { get; set; }

        public class MpnewsMain
        {
            /// <summary>
            /// 媒体ID
            /// </summary>
            public string media_id { get; set; }
        }

    }



     
}
