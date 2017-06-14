using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WeChat.WebPage.Base
{
    public class BasicMethod
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器 
        string connStr = ConfigurationManager.AppSettings["WechatDB"].ToString();
        string dllname = ConfigurationManager.AppSettings["dllname"].ToString();
        string classname = ConfigurationManager.AppSettings["classname"].ToString();

        /// <summary>
        /// 获取账号 
        /// </summary> 
        /// <param name="openID">OpenID</param>
        /// <returns>返回账号</returns>
        public string getLoginno(string openID)
        {
            string loginno = string.Empty;
            if (!string.IsNullOrEmpty("openID"))
            {
                SqlConnection conn = null;
                try
                {
                    using (conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        string sql = string.Format("select loginno from T_User where IsActive='1' and OpenID=@OpenID");
                        SqlParameter para = new SqlParameter("@OpenID", openID);
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.Add(para);
                        object temp = cmd.ExecuteScalar();
                        if (temp != null)
                        {
                            loginno = temp.ToString();
                        }
                    }

                }
                catch (Exception e)
                {
                    log.Error("BasicMethod getLoginno error", e);
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                log.Info("BasicMethod getLoginno failed: Lacking of openID");
            }
            return loginno;
        }

        /// <summary>
        /// 公众号账号密码、企业号登陆账号与企业号账号不一致时校验
        /// </summary>
        /// <param name="loginno"></param>
        /// <param name="password"></param>
        /// <param name="openid"></param>
        /// <returns></returns>
        public bool vertify(string loginno, string password, string openid)
        {
            bool result = false;
            //MyLog.WriteLog("【debug】PubBasic verify dllname :" + System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\" + dllname + ".dll");   
            //MyLog.WriteLog("【debug】PubBasic verify classname :" + classname);
            try
            {
                #region 利用反射机制调用身份验证
                Assembly assembly = Assembly.LoadFrom(System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\" + dllname + ".dll");
                Type type = assembly.GetType(classname);
                string[] param = new string[] { loginno, password };
                //MyLog.WriteLog("【debug】PubBasic verify para value:" + loginno + "----" + password + "----" + openid);
                Object obj = Activator.CreateInstance(type);
                MethodInfo mi = type.GetMethod("VertifyMethod");
                bool flag = (bool)mi.Invoke(obj, param); 
                #endregion
 

                if (flag)
                {
                    if (hasData(loginno, openid))
                    {
                        UpdateData(loginno, openid);
                    }
                    else
                    {
                        InsertData(loginno, openid);
                    }
                    result = true;
                }
            }
            catch (Exception e)
            {
                log.Error("BasicMethod verify error", e);
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 企业号登陆账号与企业号账号一致时，检验
        /// </summary>
        /// <param name="loginno"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool vertify(string loginno, string password)
        {
            bool result = false;
            try
            {
                #region 利用反射机制调用身份验证
                Assembly assembly = Assembly.LoadFrom(System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\" + dllname + ".dll");
                Type type = assembly.GetType(classname);
                string[] param = new string[] { loginno, password };
                Object obj = Activator.CreateInstance(type);
                MethodInfo mi = type.GetMethod("VertifyMethod");
                result = (bool)mi.Invoke(obj, param); 
                #endregion
            }
            catch (Exception e)
            {
                log.Error("BasicMethod verify error", e);
                result = false;
            }
            return result;
        }

        public bool ChangePassword(string loginno, string oldPassword, string newPassword)
        {
            bool result = false;

            //MyLog.WriteLog("【debug】PubBasic verify dllname :" + System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\" + dllname + ".dll");   
            //MyLog.WriteLog("【debug】PubBasic verify classname :" + classname);
            try
            {
                #region 利用反射机制调用身份验证
                Assembly assembly = Assembly.LoadFrom(System.AppDomain.CurrentDomain.BaseDirectory + @"DLL\" + dllname + ".dll");
                Type type = assembly.GetType(classname);
                string[] param = new string[] { loginno, oldPassword, newPassword };
                //MyLog.WriteLog("【debug】PubBasic verify para value:" + loginno + "----" + password + "----" + openid);
                Object obj = Activator.CreateInstance(type);
                MethodInfo mi = type.GetMethod("ChangePassword");
                bool flag = (bool)mi.Invoke(obj, param); 
                #endregion

                #region 测试
                //Vertify temp=new Vertify();
                //bool flag = temp.VertifyMethod(loginno, password);
                #endregion
            }
            catch (Exception e)
            {
                log.Error("BasicMethod ChangePassword error" ,e);
                result = false;
            }
            return result;
        }

        private bool UpdateData(string loginno, string openid)
        {
            bool result = false;
            SqlConnection conn = null;
            SqlTransaction MyTra = null;
            using (conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MyTra = conn.BeginTransaction();
                    string sql = string.Format("update T_User set IsActive ='0' where openid=@OpenID");
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Transaction = MyTra;
                    SqlParameter para = new SqlParameter("@OpenID", openid);
                    cmd.Parameters.Add(para);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("Update T_User set IsActive='1' where openid=@OpenID and loginno=@Loginno");
                    SqlParameter para2 = new SqlParameter("@Loginno", loginno);
                    cmd.Parameters.Add(para2);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        result = true;
                    }
                    MyTra.Commit();
                    conn.Close();
                }
                catch (Exception e)
                {
                    log.Error("BasicMethod UpdateData error", e);
                    MyTra.Rollback();
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        private bool InsertData(string loginno, string openid)
        {
            bool result = false;
            SqlConnection conn = null;
            SqlTransaction MyTra = null;
            using (conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MyTra = conn.BeginTransaction();
                    string sql = string.Format("update T_User set IsActive ='0' where openid=@OpenID");
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Transaction = MyTra;
                    SqlParameter para = new SqlParameter("@OpenID", openid);
                    cmd.Parameters.Add(para);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format("Insert into T_User(ID,OpenID,Loginno,IsActive) values(newid(),@OpenID,@Loginno,'1')");
                    SqlParameter para2 = new SqlParameter("@Loginno", loginno);
                    cmd.Parameters.Add(para2);
                    cmd.ExecuteNonQuery();
                    MyTra.Commit();
                    conn.Close();
                }
                catch (Exception e)
                {
                    log.Error(string.Format("【PubBasic InsertData error】：{0}", e.Message));
                    MyTra.Rollback();
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        private bool hasData(string loginno, string openid)
        {
            bool result = false;
            SqlConnection conn = null;
            using (conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = string.Format("select loginno from T_User where loginno =@Loginno and openid=@OpenID");
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlParameter para = new SqlParameter("@OpenID", openid);
                    SqlParameter para2 = new SqlParameter("@Loginno", loginno);
                    cmd.Parameters.Add(para);
                    cmd.Parameters.Add(para2);
                    object temp = cmd.ExecuteScalar();
                    if (temp != null)
                    {
                        result = true;
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    log.Error("BasicMethod hasData error", e);
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public bool DeleteData(string openid)
        {
            bool result = false;
            SqlConnection conn = null;
            SqlTransaction MyTra = null;
            using (conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    MyTra = conn.BeginTransaction();
                    string sql = string.Format("delete T_User where openid=@OpenID");
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Transaction = MyTra;
                    SqlParameter para = new SqlParameter("@OpenID", openid);
                    cmd.Parameters.Add(para);
                    cmd.ExecuteNonQuery();
                    if (cmd.ExecuteNonQuery() >= 0)
                    {
                        result = true;
                    }
                    MyTra.Commit();
                    conn.Close();
                }
                catch (Exception e)
                {
                    log.Error("BasicMethod DeleteData error", e);
                    MyTra.Rollback();
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }

        public bool hasAuthority(string loginno)
        {
            bool result = false;
            SqlConnection conn = null;
            using (conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    string sql = string.Format("select loginno from T_Admin where loginno =@Loginno and isAdmin=1");
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlParameter para = new SqlParameter("@Loginno", loginno);
                    cmd.Parameters.Add(para);
                    object temp = cmd.ExecuteScalar();
                    if (temp != null)
                    {
                        result = true;
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    log.Error("BasicMethod hasAuthority error", e);
                    if (conn != null)
                    {
                        conn.Close();
                    }
                }
            }
            return result;
        }
    }
}