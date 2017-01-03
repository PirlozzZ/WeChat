using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_WeChat.Core
{
    public class XmlUtil
    {
        public static string Serializer<T>(T t)
        {
            string temp = ToXML(t);
            string str = string.Format("<xml>{0}</xml>", temp);
            return str;
        }

        private static string ToXML<T>(T t)
        {
            string tStr = string.Empty;
            if (t == null)
            {
                return tStr;
            }
            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (properties.Length <= 0)
            {
                return tStr;
            }
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                string name = item.Name;
                object value = item.GetValue(t, null);

                if (Array.IndexOf(item.PropertyType.GetInterfaces(), typeof(IList)) > -1)
                {
                    tStr += "<" + name + ">";
                    foreach (var temp in (IEnumerable)value)
                    {
                        if (temp.GetType().IsValueType || temp.GetType().Name.StartsWith("String"))
                        {
                            tStr += temp;
                        }
                        else
                        {
                            tStr += ToXML(temp);
                        }
                    }
                    tStr += "</" + name + ">";
                }
                else
                {
                    tStr += "<" + name + ">";
                    if (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String"))
                    {
                        tStr += value;
                    }
                    else
                    {
                        tStr += ToXML(value);
                    }
                    tStr += "</" + name + ">";
                }

            }
            return tStr;

        }
    }
}