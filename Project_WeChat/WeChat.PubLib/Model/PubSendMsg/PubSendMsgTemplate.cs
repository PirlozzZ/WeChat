using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public class PubSendMsgTemplate
    {
        public PubSendMsgTemplate()
        {
            this.data = new TemplateData();
            this.topcolor = "#FF0000";
        }
        public string touser { get; set; }

        public string template_id { get; set; }

        public string url { get; set; }

        public string topcolor { get; set; }

        public TemplateData data { get; set; }

    }

    public class TemplateData
    {
        public TemplateData()
        {
            this.first = new TemplateContent();
            this.keyword1 = new TemplateContent();
            this.keyword2 = new TemplateContent();
            this.keyword3 = new TemplateContent();
            this.keyword4 = new TemplateContent();
            this.keyword5 = new TemplateContent();
            this.remark = new TemplateContent();
            this.first.color = "#173177";
            this.keyword1.color = "#173177";
            this.keyword2.color = "#173177";
            this.keyword3.color = "#173177";
            this.keyword4.color = "#173177";
            this.keyword5.color = "#173177";
            this.remark.color = "#173177";
        }
        public TemplateContent first { get; set; }
        public TemplateContent keyword1 { get; set; }
        public TemplateContent keyword2 { get; set; }
        public TemplateContent keyword3 { get; set; }
        public TemplateContent keyword4 { get; set; }
        public TemplateContent keyword5 { get; set; }
        public TemplateContent remark { get; set; }


    }

    public class TemplateContent
    {
        public string value { get; set; }
        public string color { get; set; }
    }
}