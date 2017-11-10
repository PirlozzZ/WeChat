using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VertificationLib;

namespace WeChat.FormTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //VertificationShsmu demo = new VertificationShsmu();
            //demo.VertifyMethod("183725", "092830");
            DateTime startDate = DateTime.Parse(ConfigurationManager.AppSettings["startDate"].ToString());
            string a = "1";
        }
    }
}
