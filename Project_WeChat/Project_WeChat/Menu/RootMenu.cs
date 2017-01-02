using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project_WeChat.Core
{
    public class RootMenu
    {
        public RootMenu()
        {
            this.button = new List<Menu>();
        }

        public List<Menu> button { get; set; }
    }
}
