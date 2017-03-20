using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    public class CorpUser
    {
        /// <summary>
        /// 成员UserID。对应管理端的帐号，企业内必须唯一。不区分大小写，长度为1~64个字节
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 成员名称。长度为1~64个字节
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 成员所属部门id列表,不超过20个
        /// </summary>
        public string[] department { get; set; }

        /// <summary>
        /// 职位信息。长度为0~64个字节
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 手机号码。企业内必须唯一，mobile/weixinid/email三者不能同时为空
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 性别。1表示男性，2表示女性
        /// </summary>
        public string gender { get; set; }


        /// <summary>
        /// 邮箱。长度为0~64个字节。企业内必须唯一
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 微信号。企业内必须唯一。（注意：是微信号，不是微信的名字）
        /// </summary>
        public string weixinid { get; set; }

        /// <summary>
        /// 成员头像的mediaid，通过多媒体接口上传图片获得的mediaid
        /// </summary>
        //public string avatar_mediaid { get; set; }

        /// <summary>
        /// 扩展属性。扩展属性需要在WEB管理端创建后才生效，否则忽略未知属性的赋值
        /// </summary>
        //public string extattr { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
