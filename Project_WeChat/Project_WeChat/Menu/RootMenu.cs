using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_WeChat.Menu
{
    public class RootMenu
    {
        public RootMenu()
        {
            this.button = new List<ChildMenu>();
        }

        public List<ChildMenu> button { get; set; }
    }
}
