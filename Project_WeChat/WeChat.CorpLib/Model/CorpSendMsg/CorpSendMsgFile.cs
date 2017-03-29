using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.CorpLib.Model
{

    public class CorpSendMsgFile : CorpSendMsgBase
    {
        public FileMain file { get; set; }

        public CorpSendMsgFile()
        {
            this.file = new FileMain();
            this.msgtype = "file";
        }
    }
    public class FileMain
    {
        /// <summary>
        /// 媒体文件id，可以调用上传临时素材或者永久素材接口获取
        /// </summary>
        public string media_id { get; set; }
    }
}
