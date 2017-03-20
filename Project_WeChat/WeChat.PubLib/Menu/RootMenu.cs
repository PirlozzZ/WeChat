using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChat.PubLib.Menu
{
    public class RootMenu
    {
        public RootMenu()
        {
            this.button = new List<ChildMenu>();
        }

        public List<ChildMenu> button { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
