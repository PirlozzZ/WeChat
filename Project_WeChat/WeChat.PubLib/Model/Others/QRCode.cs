using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public class QRCode
    { 
        /// <summary>
        /// 无参函数，scene_id默认是1
        /// </summary>
        public QRCode()
        {
            this.action_info = new Action_Info();
            this.action_info.scene = new Scene();
            this.action_info.scene.scene_id = 1;
            this.action_name = "QR_LIMIT_SCENE";
        }

        public QRCode(int scene_id)
        {
            this.action_info = new Action_Info();
            this.action_info.scene = new Scene();
            this.action_info.scene.scene_id = scene_id;
            this.action_name = "QR_LIMIT_SCENE";
        }

        public QRCode(string scene_str)
        {
            this.action_info = new Action_Info();
            this.action_info.scene = new Scene();
            this.action_info.scene.scene_str = scene_str;
            this.action_name = "QR_LIMIT_STR_SCENE";
        }
        /// <summary>
        /// 二维码类型，QR_SCENE为临时,QR_LIMIT_SCENE为永久,QR_LIMIT_STR_SCENE为永久的字符串参数值
        /// </summary>
        public string action_name { get; protected set; }

        public Action_Info action_info { get; set; }
    }
    /// <summary>
    /// 二维码详细信息
    /// </summary>
    public class Action_Info
    {
        public Scene scene { get; set; }
    }
    public class Scene
    {
        /// <summary>
        /// 场景值ID，临时二维码时为32位非0整型，永久二维码时最大值为100000（目前参数只支持1--100000）
        /// </summary>
        public int scene_id { get; set; }

        /// <summary>
        /// 数据创建字符串形式的二维码参数
        /// </summary>
        public string scene_str { get; set; }
    }

 
}