using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    /// <summary>
    /// 客服接口-发消息父类
    /// </summary>
    public class PubSendMsgBase
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        public string touser { get; set; }


        /// <summary>
        /// 消息类型
        /// </summary>
        public string msgtype { get; protected set; }

      

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
