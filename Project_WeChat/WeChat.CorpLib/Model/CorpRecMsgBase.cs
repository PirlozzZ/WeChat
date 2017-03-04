using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    public class CorpRecMsgBase:CorpRecAbstract
    {
        protected log4net.ILog log;

        public CorpRecMsgBase()
        {
            log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        }

        

        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public string MsgId { get; protected set; }

       

        public override string ToXML()
        {
            return Core.XmlUtil.Serializer(this);
        }

        public override void DoProcess()
        {

        }
    }
}
