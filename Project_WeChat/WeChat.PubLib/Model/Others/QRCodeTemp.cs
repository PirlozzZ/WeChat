using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public class QRCodeTemp:QRCode
    {
        /// <summary>
        /// 默认有效期7天
        /// </summary>
        /// <param name="scene_id"></param>
        public QRCodeTemp(int scene_id):this(604800, scene_id)
        {

        }

        public QRCodeTemp(int expire_seconds, int scene_id)
        {
            this.expire_seconds = expire_seconds;
            this.action_name = "QR_SCENE";
            this.action_info.scene.scene_id = scene_id;
        }
        /// <summary>
        /// 该二维码有效时间，以秒为单位。 最大不超过2592000（即30天），此字段如果不填，则默认有效期为30秒。
        /// </summary>
        public int expire_seconds { get; set; }
    }
}