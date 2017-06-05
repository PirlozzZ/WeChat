using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.CorpLib.Model
{
    public class CorpOAuth_UserDetail
    {
        /// <summary>
        /// 成员UserID
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 成员姓名
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 成员所属部门
        /// </summary>
        public string department { get; set; }

        /// <summary>
        /// 职位信息
        /// </summary>
        public string position { get; set; }

        /// <summary>
        /// 成员手机号，仅在用户同意snsapi_privateinfo授权时返回
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 性别。0表示未定义，1表示男性，2表示女性
        /// </summary>
        public string gender { get; set; }

        /// <summary>
        /// 成员邮箱，仅在用户同意snsapi_privateinfo授权时返回
        /// </summary>
        public string email { get; set; }

        /// <summary>
        /// 头像url。注：如果要获取小图将url最后的”/0”改成”/64”即可
        /// </summary>
        public string avatar { get; set; }
    }

}
