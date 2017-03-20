using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.PubLib.Model
{
    public class PubResMsgBase
    {
        /// <summary>
        /// 接收方帐号（收到的OpenID）
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        /// 开发者微信号
        /// </summary>
        public string FromUserName { get; set; }

        /// <summary>
        /// 消息创建时间 （整型）
        /// </summary>
        public string CreateTime { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MsgType { get; protected set; }

        public string ToXML()
        {
            return Core.XmlUtil.Serializer(this);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
}
