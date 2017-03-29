using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.CorpLib.Model
{
    public class CorpDepartment
    {
        /// <summary>
        /// 部门名称。长度限制为32个字（汉字或英文字母），字符不能包括\:*?"<>｜
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 成父亲部门id。根部门id为1
        /// </summary>
        public string parentid { get; set; }

        /// <summary>
        /// 在父部门中的次序值。order值小的排序靠前。
        /// </summary>
        public string order { get; set; }

        /// <summary>
        /// 部门id，整型。指定时必须大于1，不指定时则自动生成
        /// </summary>
        public string id { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
