using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace WeChat.SvcApp
{
    public partial class SvcPub : ServiceBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
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
            string connStr = ConfigurationManager.AppSettings["sqlConn"].ToString();
            string startDay = ConfigurationManager.AppSettings["startDate"].ToString(); 
        }
    }
}
