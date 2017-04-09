using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.PubLib.Model
{
    public class OAuth_AccessToken
    {
        protected log4net.ILog log;

        public OAuth_AccessToken(string sMsg)
        {
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(sMsg);
                this.access_token = jo["access_token"].ToString();
                this.expires_in = jo["expires_in"].ToString();
                this.refresh_token = jo["refresh_token"].ToString();
                this.openid = jo["openid"].ToString();
                this.scope = jo["scope"].ToString();
                object temp = jo["unionid"];               
            }
            catch (Exception e)
            {
                log.Error("OAuth_AccessToken error!", e);
            }
        }

        /// <summary>
        /// 网页授权接口调用凭证,注意：此access_token与基础支持的access_token不同
        /// </summary>
        public string access_token { get; set; }

        /// <summary>
        /// access_token接口调用凭证超时时间，单位（秒）
        /// </summary>
        public string expires_in { get; set; }

        /// <summary>
        /// 用户刷新access_token 
        /// </summary>
        public string refresh_token { get; set; }

        /// <summary>
        /// 用户唯一标识，请注意，在未关注公众号时，用户访问公众号的网页，也会产生一个用户和公众号唯一的OpenID
        /// </summary>
        public string openid { get; set; }

        /// <summary>
        /// 用户授权的作用域，使用逗号（,）分隔
        /// </summary>
        public string scope { get; set; }

        
}
}