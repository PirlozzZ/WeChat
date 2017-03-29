using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChat.CorpLib.Model
{

    /// <summary>
    /// 异步任务完成事件推送类
    /// </summary>
    public class CorpRecEventBatch_job_result : CorpRecEventBase
    {
        /// <summary>
        /// 异步任务完成事件
        /// </summary>
        public static event WechatEventHandler<CorpRecEventBatch_job_result> OnEventBatch_job_result;        //声明事件

        public CorpRecEventBatch_job_result(string sMsg)
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
                this.AgentID = root["AgentID"].InnerText;
                XmlNode nodeBatchJob = root["BatchJob"];
                this.batchJob = new BatchJob();
                this.batchJob.JobId = nodeBatchJob["JobId"].InnerText;
                this.batchJob.JobType = nodeBatchJob["JobType"].InnerText;
                this.batchJob.ErrCode = nodeBatchJob["ErrCode"].InnerText;
                this.batchJob.ErrMsg = nodeBatchJob["ErrMsg"].InnerText;
            }
            catch (Exception e)
            {
                log.Error("CorpRecEventBatch_job_result", e);
            }
        }

        public override string DoProcess()
        {
            
            string strResult = string.Empty;
            if (OnEventBatch_job_result != null)
            { //如果有对象注册 
                strResult=OnEventBatch_job_result(this);  //调用所有注册对象的方法
            }
            return strResult;
        }

        /// <summary> 
        /// 事件KEY值，由开发者在创建菜单时设定L
        /// </summary>
        public BatchJob batchJob { get; private set; }

         
        public class BatchJob
        {
            /// <summary>
            /// 异步任务id，最大长度为64字符
            /// </summary>
            public string JobId { get; set; }

            /// <summary>
            /// 操作类型，字符串，目前分别有：
            ///1. sync_user(增量更新成员)
            ///2. replace_user(全量覆盖成员)
            ///3. invite_user(邀请成员关注)
            ///4. replace_party(全量覆盖部门)
            /// </summary>
            public string JobType { get; set; }

            /// <summary>
            /// 返回码
            /// </summary>
            public string ErrCode { get; set; }

            /// <summary>
            /// 对返回码的文本描述内容
            /// </summary>
            public string ErrMsg { get; set; }

             
        }
    }
}
