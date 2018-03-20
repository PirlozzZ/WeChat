using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace VertificationLib.Base
{
    
    public class LDAPHelper
    {
        private DirectoryEntry _objDirectoryEntry;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LADPath">ldap的地址，例如"LDAP://***.***.48.110:389/dc=***,dc=com"</param>
        /// <param name="authUserName">连接用户名，例如"cn=root,dc=***,dc=com"</param>
        /// <param name="authPWD">连接密码</param>
        public bool OpenConnection(string LADPath, string authUserName, string authPWD)
        {    //创建一个连接 
            try
            {
                _objDirectoryEntry = new DirectoryEntry(LADPath, authUserName, authPWD, AuthenticationTypes.None);

                if (null == _objDirectoryEntry)
                {
                    return false;
                }
                else if (_objDirectoryEntry.Properties != null && _objDirectoryEntry.Properties.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;
        }

        /// <summary>
        /// 检测一个用户和密码是否正确
        /// </summary>
        /// <param name="strLDAPFilter">(|(uid= {0})(cn={0}))</param>
        /// <param name="TestUserID">testuserid</param>
        /// <param name="TestUserPwd">testuserpassword</param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public bool CheckUidAndPwd(string strLDAPFilter, string TestUserID, string TestUserPwd, ref string ErrorMessage)
        {
            bool blRet = false;
            try
            {
                //创建一个检索
                DirectorySearcher deSearch = new DirectorySearcher(_objDirectoryEntry);
                //过滤名称是否存在
                deSearch.Filter = strLDAPFilter;
                deSearch.SearchScope = SearchScope.Subtree;

                //find the first instance 
                SearchResult objSearResult = deSearch.FindOne();

                //如果用户密码为空
                if (string.IsNullOrEmpty(TestUserPwd))
                {
                    if (null != objSearResult && null != objSearResult.Properties && objSearResult.Properties.Count > 0)
                    {
                        blRet = true;
                    }
                }
                else if (null != objSearResult && !string.IsNullOrEmpty(objSearResult.Path))
                {
                    //获取用户名路径对应的用户uid
                    int pos = objSearResult.Path.LastIndexOf('/');
                    string uid = objSearResult.Path.Remove(0, pos + 1);
                    DirectoryEntry objUserEntry = new DirectoryEntry(objSearResult.Path, uid, TestUserPwd, AuthenticationTypes.None);
                    if (null != objUserEntry && objUserEntry.Properties.Count > 0)
                    {
                        blRet = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "检测异常：" + ex.StackTrace;
            }
            finally
            {
                if (null != _objDirectoryEntry)
                {
                    _objDirectoryEntry.Close();
                }
            }
            return blRet;
        }


        /// <summary>
        /// 关闭连接
        /// </summary>
        public void closeConnection()
        {
            if (null != _objDirectoryEntry)
            {
                _objDirectoryEntry.Close();
            }
        }
    }
}
