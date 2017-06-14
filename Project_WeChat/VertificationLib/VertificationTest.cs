using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VertificationLib
{
    public class VertificationTest : IVertification
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器

        public bool VertifyMethod(string loginno, string password)
        {
            log.Debug("VertificationTest:" + loginno + password);
            return true;
        }
    }
}
