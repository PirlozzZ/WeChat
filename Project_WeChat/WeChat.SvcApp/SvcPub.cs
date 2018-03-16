using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using WeChat.PubLib.Core;
using WeChat.PubLib.Model;

namespace WeChat.SvcApp
{
    public partial class SvcPub : ServiceBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        string dllname = ConfigurationManager.AppSettings["dllname"].ToString();
        string classname = ConfigurationManager.AppSettings["classname"].ToString();
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
             
            try
            {
                #region 利用反射机制调用身份验证
               
                Assembly assembly = Assembly.LoadFrom(System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\" + dllname + ".dll");
               
                Type type = assembly.GetType(classname);
               
                Object obj = Activator.CreateInstance(type);
                
                MethodInfo mi = type.GetMethod("MsgPushRuleMethod");
             
                mi.Invoke(obj, null);
                #endregion
            }
            catch (Exception err)
            {
                log.Error(System.AppDomain.CurrentDomain.BaseDirectory+dllname+ "|" + classname + "|SvcPub timerOverDayHourFroze_Elapsed:", err); 
            } 

            
        }
    }
}
