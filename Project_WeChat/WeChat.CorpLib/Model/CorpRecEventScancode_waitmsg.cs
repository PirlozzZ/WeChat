using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{
    /// <summary>
    /// 扫码推事件且弹出“消息接收中”提示框的事件推送类
    /// </summary>
    public class CorpRecEventScancode_waitmsg : CorpRecEventBase
    {
        //扫码推事件且弹出“消息接收中”提示框的事件
        public static event WechatEventHandler<CorpRecEventScancode_waitmsg> OnEventScancode_waitmsg;        //声明事件

        public CorpRecEventScancode_waitmsg(string sMsg)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(sMsg);
                XmlNode root = doc.FirstChild;
                this.ToUserName = root["ToUserName"].InnerText;
                this.FromUserName = root["FromUserName"].InnerText;
                this.CreateTime = root["CreateTime"].InnerText;
                this.MsgType = root["MsgType"].InnerText;
                this.Event = root["Event"].InnerText;
                this.EventKey = root["EventKey"].InnerText;
                this.AgentID = root["AgentID"].InnerText;
                this.scanCodeInfo = new ScanCodeInfo();
                XmlNode nodeScanCodeInfo = root["ScanCodeInfo"];
                scanCodeInfo.ScanType = nodeScanCodeInfo["ScanType"].InnerText;
                scanCodeInfo.ScanResult = nodeScanCodeInfo["ScanResult"].InnerText;
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventScancode_waitmsg", e);
            }
        }

        public override void DoProcess()
        {
            if (OnEventScancode_waitmsg != null)
            { //如果有对象注册 
                OnEventScancode_waitmsg(this);  //调用所有注册对象的方法
            }
        }

        /// <summary> 
        /// 事件KEY值，由开发者在创建菜单时设定L
        /// </summary>
        public string EventKey { get; private set; }

        /// <summary> 
        /// 扫描信息
        /// </summary>
        public ScanCodeInfo scanCodeInfo { get; private set; }

        public class ScanCodeInfo
        {
            /// <summary> 
            /// 扫描类型，一般是qrcode
            /// </summary>
            public string ScanType { get; set; }

            /// <summary> 
            /// 扫描结果，即二维码对应的字符串信息
            /// </summary>
            public string ScanResult { get; set; }
        }
    }
}
