using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 自动回复消息父类
    /// </summary>
    public class CorpResMsgBase
    {
        /// <summary>
        /// 成员UserID
        /// </summary>
        public string ToUserName { get; set; }

        /// <summary>
        ///  企业号CorpID
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

        //public string ToJson()
        //{
        //    return JsonConvert.SerializeObject(this);
        //}
    }
}
