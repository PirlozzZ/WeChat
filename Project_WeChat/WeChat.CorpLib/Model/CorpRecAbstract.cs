using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    public abstract class CorpRecAbstract
    {
        public delegate void WechatEventHandler<in T>(T instanse);   //声明委托

        public abstract void DoProcess();

        public abstract string ToXML();
    }
}
