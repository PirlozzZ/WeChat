using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    public class CorpRecEventBase : CorpRecAbstract
    {
        protected log4net.ILog log;

        public CorpRecEventBase()
        {
            log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string Event { get; protected set; }

        public override string ToXML()
        {
            return Core.XmlUtil.Serializer(this);
        }

        public override void DoProcess()
        {

        }
    }
}
