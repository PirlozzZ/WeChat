using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Model
{
    public class MaterialCondition
    {
        public MaterialCondition()
        {

        }

        public MaterialCondition(MaterialTypeEnum type,int offset,int count)
        {
            this.type = type.ToString();
            this.offset = offset;
            this.count = count;
        } 

        /// <summary>
        /// 素材的类型
        /// </summary>
        public string type { get; set;}

        /// <summary>
        /// 从全部素材的该偏移位置开始返回，0表示从第一个素材 返回
        /// </summary>
        public int offset { get; set; }

        /// <summary>
        /// 返回素材的数量，取值在1到20之间
        /// </summary>
        public int count { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public enum MaterialTypeEnum
    {
        image,
        video,
        voice,
        news
    }
}
