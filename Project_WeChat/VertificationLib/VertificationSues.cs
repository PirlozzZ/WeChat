using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VertificationLib.Base;

namespace VertificationLib
{
    public class VertificationSues : IVertification
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器
        public bool VertifyMethod(string loginno, string password)
        {
            bool result = false;
            try {
                LDAPHelper objldap = new LDAPHelper();
                string strLDAPPath = "LDAP://202.121.127.222/dc=sues,dc=edu,dc=cn";
                string strLDAPAdminName = "uid=ldapsus,ou=People,dc=sues,dc=edu,dc=cn";
                //string strLDAPAdminName = "uid=ldapsus,cn=People,dc=sues,dc=edu,dc=cn";
                string strLDAPAdminPwd = "sus1qaz";
                string strLDAPFilter = string.Format("(uid= {0})");
                bool blRet = objldap.OpenConnection(strLDAPPath, strLDAPAdminName, strLDAPAdminPwd);
                string strMsg = string.Empty;

                if (blRet)
                {
                    result = objldap.CheckUidAndPwd(strLDAPFilter, loginno, password, ref strMsg);
                    if (!result)
                    {
                        log.Info("VertificationSues VertifyMethod failed:" + strMsg);
                    }
                }
            }
            catch(Exception e)
            {
                log.Error("VertificationSues VertifyMethod error!", e);
            }
            return result;
        }
    }

}
