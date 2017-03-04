using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public  class PubRecEventBase: PubRecAbstract
    {
        protected log4net.ILog log;

        public PubRecEventBase()
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