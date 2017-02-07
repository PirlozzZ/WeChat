using System.Collections.Generic;

namespace Project_WeChat.Menu
{
    public class ChildMenu
    {
        public ChildMenu(string name) : this(name,MenuTypeEnum.click,string.Empty)
        {

        }
        public ChildMenu(string name, MenuTypeEnum type,string value)
        {
            this.type = type.ToString();
            this.name = name;
            if(type== MenuTypeEnum.view)
            {
                url = value;
            }
            else
            {
                key = value;
            }
            this.sub_button = new List<ChildMenu>();
        }

        public string name { get; set; }
        public string key { get; set; }
        public string url { get; set; }
        public List<ChildMenu> sub_button { get; set; }
        public string type { get; private set; }
        /// <summary>
        /// 微信菜单类型
        /// </summary>
        public enum MenuTypeEnum
        {
            /// <summary>
            /// 点击推事件
            /// </summary>
            click,
            ///// <summary>
            ///// 跳转url
            ///// </summary>
            view,
            /// <summary>
            /// 扫码推事件
            /// </summary>
            scancode_push,
            /// <summary>
            /// 扫码推事件且弹出“消息接收中”提示框
            /// </summary>
            scancode_waitmsg,
            /// <summary>
            /// 弹出系统拍照发图
            /// </summary>
            pic_sysphoto,
            /// <summary>
            /// 弹出拍照或者相册发图
            /// </summary>
            pic_photo_or_album,
            /// <summary>
            /// 弹出微信相册发图器
            /// </summary>
            pic_weixin,
            /// <summary>
            /// 弹出地理位置选择器
            /// </summary>
            location_select
        }
    }
}
