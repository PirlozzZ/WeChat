using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using WeChat.PubLib.Core;
using WeChat.PubLib.Model;

namespace MsgPushRuleLib
{
    public class MsgPushRuleSues : IMsgPushRule
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        string connStr = ConfigurationManager.AppSettings["sqlConn"].ToString();
        DateTime startDate = DateTime.Parse(ConfigurationManager.AppSettings["startDate"].ToString());
        string templateID = ConfigurationManager.AppSettings["templateID"].ToString();
        string sign = ConfigurationManager.AppSettings["sign"].ToString();
        static PubCore core;

        public MsgPushRuleSues()
        {
            core = new PubCore(sign, PubCore.ServerType.OtherServer);
        }

        public void MsgPushRuleMethod()
        {
            DateTime now = DateTime.Now;
            try
            {
                if (DateTime.Compare(startDate, now) < 0)
                {
                    string sql = string.Format("select * from [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] a left join [WechatDB].[dbo].[T_User] b on a.Touser=b.Loginno where Sendstate=0  and datediff(day,Operationtime,'{0}')<=0", startDate);
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        PubSendMsgTemplate template = new PubSendMsgTemplate();
                        template.template_id = templateID;

                        sda.Fill(dt);
                        foreach (DataRow item in dt.Rows)
                        {
                            string flag = item["isActive"] == null ? "0" : item["isActive"].ToString();
                            if ("1".Equals(flag.Trim()))
                            {
                                template.touser = item["OpenID"].ToString();
                                template.data.keyword1.value = item["Field1"].ToString();
                                template.data.keyword2.value = item["Remark"].ToString();
                                template.data.keyword3.value = item["Field8"].ToString();
                                template.data.keyword4.value = item["Field6"].ToString();
                                template.data.keyword5.value = item["Field5"].ToString();
                                if ("驳回".Equals(item["Field2"].ToString()))
                                {
                                    template.data.first.value = string.Format("您好，你的预约审核被{0}", item["Field2"].ToString());
                                    template.data.remark.value = string.Format("财务处已驳回您的报销单，请登陆网上报销预约系统查看驳回窗口和驳回理由，请至该窗口拿回报销单。");
                                }
                                else if ("完成".Equals(item["Field2"].ToString()))
                                {
                                    template.data.first.value = string.Format("您好，你的预约审核{0}", item["Field2"].ToString());
                                    template.data.remark.value = string.Format("将在一周内完成打款，若有疑问请至财务处咨询。");
                                }
                                else
                                {
                                    template.data.first.value = string.Format("您好，你的预约审核{0}", item["Field2"].ToString());
                                    template.data.remark.value = string.Format("财务处已接收了您的报销单，请等待完成。");
                                }


                                if (!core.SendTemplate(template))
                                {
                                    log.Info(string.Format("Send Template Failed：{0}——{1}", item["First"].ToString(), now));
                                    cmd.CommandText = string.Format("update [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] set Sendstate=3,Sendtime='{0}' where Field1={1}", now, item["Field1"].ToString());
                                    cmd.ExecuteNonQuery();
                                }
                                else
                                {
                                    cmd.CommandText = string.Format("update [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] set Sendstate=1,Sendtime='{0}' where id='{1}'", now, item["ID"].ToString());
                                    if (cmd.ExecuteNonQuery() != 1)
                                    {
                                        log.Info(string.Format("Send Template SysnDB Failed：{0}", item["Field1"].ToString()));
                                        cmd.CommandText = string.Format("update [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] set Sendstate=2,Sendtime='{0}' where Field1={1}", now, item["Field1"].ToString());
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                            }
                            else
                            {
                                log.Info(string.Format("Send Template Failed ：No Login {0}——{1}", item["First"].ToString(), now));
                                cmd.CommandText = string.Format("update [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] set Sendstate=4,Sendtime='{0}' where Field1={1}", now, item["Field1"].ToString());
                                cmd.ExecuteNonQuery();
                            }

                        }
                        conn.Close();
                    }
                }
            }
            catch (Exception err)
            {
                log.Error("MsgPushRuleSues MsgPushRuleMethod:", err);
            }
        }
    }
}
