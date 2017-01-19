using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_WeChat.Model
{
    public class PubRecEventBase:PubRecMsgBase
    {
        /// <summary>
        /// 消息id，64位整型
        /// </summary>
        public new string MsgId { get; private set; }

        /// <summary>
        /// 事件类型
        /// </summary>
        public string Event { get; protected set; }

        public delegate void ProcessHandler();   //声明委托
        public static event ProcessHandler ProcessEvent;        //声明事件

        public void DoProcess()
        {
            if (ProcessEvent != null)
            { //如果有对象注册
                ProcessEvent();  //调用所有注册对象的方法
            }
        }
    }
}