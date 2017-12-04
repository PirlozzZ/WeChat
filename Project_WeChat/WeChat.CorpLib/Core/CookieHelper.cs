using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Configuration;

namespace WeChat.CorpLib.Core
{
    /// <summary>
    /// 操作cookie
    /// </summary>
    public class CookieHelper
    {

        #region "定义加密字串变量"
        private SymmetricAlgorithm mCSP;  //声明对称算法变量
        private const string CIV = "seaskysh2015";//密钥
        private const string CKEY = "FwGQWRRgKCI=";//初始化向量
        #endregion

        public CookieHelper()
        {
            mCSP = new DESCryptoServiceProvider();  //定义访问数据加密标准 (DES) 算法的加密服务提供程序 (CSP) 版本的包装对象,此类是SymmetricAlgorithm的派生类
        }

        /// <summary>
        /// Cookies赋值
        /// </summary>
        /// <param name="strName">主键</param>
        /// <param name="strValue">键值</param>
        /// <param name="strDay">有效天数</param>
        /// <returns></returns>
        public bool setCookie(string strName, string strValue, int strDay)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(strDay);
                Cookie.Value = strValue;
                HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch (Exception e)
            {
                throw e; 
            }
        }

        /// <summary>
        /// 读取Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>
        public string getCookie(string strName)
        {
            HttpCookie Cookie = HttpContext.Current.Request.Cookies[strName];
            if (Cookie != null)
            {
                return Cookie.Value.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 删除Cookies
        /// </summary>
        /// <param name="strName">主键</param>
        /// <returns></returns>
        public bool delCookie(string strName)
        {
            try
            {
                HttpCookie Cookie = new HttpCookie(strName);
                Cookie.Value = null;
                //Cookie.Domain = ".xxx.com";//当要跨域名访问的时候,给cookie指定域名即可,格式为.xxx.com
                Cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(Cookie);
                return true;
            }
            catch(Exception e)
            { 
                throw e;
            }
        }

        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="Value">需加密的字符串</param>
        /// <returns></returns>
        public string EncryptString(string Value)
        {
            ICryptoTransform ct; //定义基本的加密转换运算
            MemoryStream ms=null; //定义内存流
            CryptoStream cs; //定义将内存流链接到加密转换的流
            byte[] byt;
            try
            {
                //CreateEncryptor创建(对称数据)加密对象
                ct = mCSP.CreateEncryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV)); //用指定的密钥和初始化向量创建对称数据加密标准
                byt = Encoding.UTF8.GetBytes(Value); //将Value字符转换为UTF-8编码的字节序列
                ms = new MemoryStream(); //创建内存流
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write); //将内存流链接到加密转换的流
                cs.Write(byt, 0, byt.Length); //写入内存流
                cs.FlushFinalBlock(); //将缓冲区中的数据写入内存流，并清除缓冲区
                cs.Close(); //释放内存流
            }
            catch (Exception e)
            {
                throw e;
            }
            return Convert.ToBase64String(ms.ToArray()); //将内存流转写入字节数组并转换为string字符
            
        }

        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="Value">要解密的字符串</param>
        /// <returns>string</returns>
        public string DecryptString(string Value)
        {
            ICryptoTransform ct; //定义基本的加密转换运算
            MemoryStream ms=null; //定义内存流
            CryptoStream cs; //定义将数据流链接到加密转换的流
            byte[] byt;
            try
            {
                ct = mCSP.CreateDecryptor(Convert.FromBase64String(CKEY), Convert.FromBase64String(CIV)); //用指定的密钥和初始化向量创建对称数据解密标准
                byt = Convert.FromBase64String(Value); //将Value(Base 64)字符转换成字节数组
                ms = new MemoryStream();
                cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
                cs.Write(byt, 0, byt.Length);
                cs.FlushFinalBlock();
                cs.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            return Encoding.UTF8.GetString(ms.ToArray()); //将字节数组中的所有字符解码为一个字符串
        }
    }




}
