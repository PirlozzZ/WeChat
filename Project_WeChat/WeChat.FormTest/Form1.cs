using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VertificationLib;
using WeChat.PubLib.Core;
using WeChat.PubLib.Model;

namespace WeChat.FormTest
{
    public partial class Form1 : Form
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        string connStr = ConfigurationManager.AppSettings["sqlConn"].ToString();
        DateTime startDate = DateTime.Parse(ConfigurationManager.AppSettings["startDate"].ToString());
        string templateID = ConfigurationManager.AppSettings["templateID"].ToString();
        string sign = ConfigurationManager.AppSettings["sign"].ToString();
        PubCore core;
        public Form1()
        {
            InitializeComponent();
            core = new PubCore(sign, PubCore.ServerType.OtherServer);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string a = "42.0000";
            a= decimal.Round(decimal.Parse(a),2).ToString();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
