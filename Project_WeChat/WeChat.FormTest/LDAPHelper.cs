using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;

namespace WeChat.FormTest
{
    public class LDAPHelper
    {
        // Fields
        private DirectoryEntry _objDirectoryEntry;

        // Methods
        public bool CheckUidAndPwd(string strLDAPFilter, string TestUserID, string TestUserPwd, ref string ErrorMessage)
        {
            bool flag = false;
            try
            {
                SearchResult result = new DirectorySearcher(this._objDirectoryEntry) { Filter = strLDAPFilter, SearchScope = SearchScope.Subtree }.FindOne();
                if (string.IsNullOrEmpty(TestUserPwd))
                {
                    if (((result != null) && (result.Properties != null)) && (result.Properties.Count > 0))
                    {
                        flag = true;
                    }
                    return flag;
                }
                if ((result == null) || string.IsNullOrEmpty(result.Path))
                {
                    return flag;
                }
                int num = result.Path.LastIndexOf('/');
                string username = result.Path.Remove(0, num + 1);
                DirectoryEntry entry = new DirectoryEntry(result.Path, username, TestUserPwd, AuthenticationTypes.None);
                if ((entry != null) && (entry.Properties.Count > 0))
                {
                    flag = true;
                }
            }
            catch (Exception exception)
            {
                if (null != this._objDirectoryEntry)
                {
                    this._objDirectoryEntry.Close();
                }
                ErrorMessage = "检测异常：" + exception.StackTrace;
            }
            return flag;
        }

        public void closeConnection()
        {
            if (null != this._objDirectoryEntry)
            {
                this._objDirectoryEntry.Close();
            }
        }

        public bool OpenConnection(string LADPath, string authUserName, string authPWD)
        {
            this._objDirectoryEntry = new DirectoryEntry(LADPath, authUserName, authPWD, AuthenticationTypes.None);
            if (null == this._objDirectoryEntry)
            {
                return false;
            }
            return ((this._objDirectoryEntry.Properties != null) && (this._objDirectoryEntry.Properties.Count > 0));
        }
    }
}
