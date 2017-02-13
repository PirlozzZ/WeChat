using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public abstract class PubRecAbstract
    {
        public delegate void WechatEventHandler<in T>(T instanse);   //声明委托

        public abstract void DoProcess();

        public abstract string ToXML();
    }
}