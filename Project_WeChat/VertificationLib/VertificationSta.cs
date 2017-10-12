using Seasky.SFP.Common.Model;
using Seasky.SFP.Common.PlatAdmin.CommonBLL;
using Seasky.StandardLib.MyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VertificationLib
{
    public class VertificationSta : IVertification
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Log.Logging");//获取一个日志记录器
        public bool VertifyMethod(string loginno, string password)
        {
            bool result = false;
            #region 从财务平台统一校验帐号
            try
            {
                string o_TokenEncryp = string.Empty;
                string p_DoMain = string.Empty;
                CommonConnStr.ChangeConfigKey("ShareSqlConn");
                Com_User_En_BLL bll = new Com_User_En_BLL();
                ResultInfo valiResult = null;
                if (string.IsNullOrEmpty(password))
                {
                    valiResult = bll.SFPLoginVerify_SSO(loginno, out o_TokenEncryp, out p_DoMain);
                }
                else
                {
                    valiResult = bll.SFPLoginVerify(loginno, password, out o_TokenEncryp, out p_DoMain);
                }
                if (valiResult.Successed)
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                log.Debug("VertificationSta VertifyMethod error!" + e.Message);
            }

            #endregion
            return result;
        }
    }
}
