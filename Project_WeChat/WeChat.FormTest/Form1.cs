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
            //string a = "42.0000";
            //a= decimal.Round(decimal.Parse(a),2).ToString();
            List<string> temp = new List<string>();
            temp.Add("a1");
            temp.Add("a2");
            temp.Add("v1");
            bool result1=temp.Contains("a");
            bool result2 = temp.Contains("a1");
            temp.RemoveAll(x =>x.StartsWith("a"));


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string strLDAPFilter = string.Format(this.txtFilter.Text, this.txtUserName.Text.Trim());
            string text = this.txtUserName.Text;
            string testUserPwd = this.txtPwd.Text;
            LDAPHelper helper = new LDAPHelper();
            string lADPath = this.txtLDAP.Text;
            string authUserName = this.txtLUserName.Text;
            string authPWD = this.txtLPwd.Text;
            string errorMessage = "";
            if (helper.OpenConnection(lADPath, authUserName, authPWD))
            {
                bool flag = helper.CheckUidAndPwd(strLDAPFilter, text, testUserPwd, ref errorMessage);
                if (flag)
                {
                    errorMessage = "检测用户名" + text + "和密码" + testUserPwd + "成功";
                }
                else if (!(flag || !string.IsNullOrEmpty(errorMessage)))
                {
                    errorMessage = "检测用户名" + text + "和密码" + testUserPwd + "失败";
                }
            }
            //this.txtLog.Text = DateTime.Now.ToString() + ":" + errorMessage + "\r\n\r\n" + this.txtLog.Text;
            MessageBox.Show(errorMessage);

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
