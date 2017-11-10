using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using WeChat.PubLib.Core;
using WeChat.PubLib.Model;

namespace WeChat.SvcApp
{
    public partial class SvcPub : ServiceBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        string connStr = ConfigurationManager.AppSettings["sqlConn"].ToString();
        DateTime startDate = DateTime.Parse(ConfigurationManager.AppSettings["startDate"].ToString());
        string templateID = ConfigurationManager.AppSettings["templateID"].ToString();
        string sign = ConfigurationManager.AppSettings["sign"].ToString();
        private System.Timers.Timer _timerOverDayHourFroze;
        public SvcPub()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timerOverDayHourFroze = new System.Timers.Timer();
            _timerOverDayHourFroze.Interval = double.Parse(ConfigurationManager.AppSettings["Interval"].ToString());
            _timerOverDayHourFroze.Enabled = true;
            _timerOverDayHourFroze.Start();
            _timerOverDayHourFroze.Elapsed += new System.Timers.ElapsedEventHandler(timerOverDayHourFroze_Elapsed); ;
            log.Info(string.Format("【服务开始】"));
        }

        protected override void OnStop()
        {
            log.Info(string.Format("【服务结束】"));
        }

        private void timerOverDayHourFroze_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            
            DateTime now = DateTime.Now;
            PubCore core = new PubCore(sign);
            if (DateTime.Compare(startDate, now) < 0)
            {
                string sql = string.Format("select * from [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] where Sendstate=0");
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
                        template.data.first.value = string.Format("您好，你的预约审核{0}", item["Field2"].ToString());
                        template.data.keyword1.value = item["Field1"].ToString();
                        template.data.keyword2.value = item["Remark"].ToString();
                        template.data.keyword3.value = item["Field8"].ToString();
                        template.data.keyword4.value = item["Field6"].ToString();
                        template.data.keyword5.value = item["Field5 "].ToString();
                        template.data.remark.value = item["Field4 "].ToString();
                        if (!core.SendTemplate(template))
                        {
                            log.Info(string.Format("Send Template Failed：{0}——{1}", item["Empname"].ToString(), now));
                        }
                        else
                        {
                            cmd.CommandText = string.Format("update [SFP_Middle].[dbo].[Mid_O_ClaimsOrder] set Sendstate=1 where Field1={0}", item["Field1"].ToString());
                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                log.Info(string.Format("Send Template SysnDB Failed：{0}", item["Field1"].ToString()));
                            }
                        }
                    }
                    conn.Close();
                }
            }
        }
    }
}
