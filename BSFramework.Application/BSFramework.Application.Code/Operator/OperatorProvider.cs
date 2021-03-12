using BSFramework.Cache.Factory;
using BSFramework.Util;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.IO;
using System.Web;
using System.Web.Security;

namespace BSFramework.Application.Code
{
    /// <summary>
    /// 描 述：当前操作者回话
    /// </summary>
    public class OperatorProvider : OperatorIProvider
    {
        #region 静态实例
        /// <summary>
        /// 当前提供者
        /// </summary>
        public static OperatorIProvider Provider
        {
            get { return new OperatorProvider(); }
        }
        /// <summary>
        /// 给app调用
        /// </summary>
        public static string AppUserId
        {
            set;
            get;
        }
        #endregion

        private string systemName = Config.GetValue("SystemName");
        /// <summary>
        /// 秘钥
        /// </summary>
        private string LoginUserKey = "LoginUserKey";
        /// <summary>
        /// 登陆提供者模式:Session、Cookie 
        /// </summary>
        private string LoginProvider = Config.GetValue("LoginProvider");
        /// <summary>
        /// 写入登录信息
        /// </summary>
        /// <param name="user">成员信息</param>
        public virtual void AddCurrent(Operator userData)
        {

            try
            {
                //if (LoginProvider == "Cookie")
                //{
                //    CacheFactory.Cache().WriteCache(userData.DataAuthorize, userData.UserId + "_" + LoginUserKey, userData.LogTime.AddHours(12));
                //    //WebHelper.WriteCookie(LoginUserKey, DESEncrypt.Encrypt(new { UserId = userData.UserId, Account = userData.Account, UserName = userData.UserName, IsSystem = userData.IsSystem, Token = userData.Token, Code = userData.Code, DeptCode = userData.DeptCode, OrganizeCode = userData.OrganizeCode, OrganizeId = userData.OrganizeId, DeptId = userData.DeptId, RoleName = userData.RoleName, PostName = userData.PostName, DeptName = userData.DeptName, OrganizeName = userData.OrganizeName, Password = userData.Password, Secretkey = userData.Secretkey }.ToJson()));
                //}
                //else
                //{
                //    WebHelper.WriteSession(LoginUserKey, DESEncrypt.Encrypt(userData.ToJson()));
                //}
                if (LoginProvider == "AppClient")
                {
                    WebHelper.WriteCookie(systemName + "_" + AppUserId, userData.UserId);
                }
                FormsAuth.SignIn(systemName + "_" + userData.UserId, userData, 2 * 60);
                CacheFactory.Cache().WriteCache(userData, systemName + "_" + userData.UserId, userData.LogTime.AddHours(24));
                CacheFactory.Cache().WriteCache(userData.Token, systemName + "_" + userData.UserId + "_" + LoginUserKey, userData.LogTime.AddHours(12));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public virtual string GetUserId()
        {
            string userId = "";
            string json = HttpContext.Current.Request["json"];
            if (string.IsNullOrEmpty(json))
            {
                Stream stream = HttpContext.Current.Request.InputStream;
                StreamReader sr = new StreamReader(stream);
                json = sr.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(json))
            {
                dynamic dy = JsonConvert.DeserializeObject<ExpandoObject>(json.ToLower());
                if (json.Contains("json"))
                {
                    userId = dy.json.userid.ToString();
                }
                else
                {
                    userId = dy.userid.ToString();
                }
            }
            return userId;
        }
        /// <summary>
        /// 当前用户
        /// </summary>
        /// <returns></returns>
        public virtual Operator Current()
        {



            try
            {
                Operator user = new Operator();

                if (LoginProvider == "AppClient")
                {
                    user = CacheFactory.Cache().GetCache<Operator>(systemName + "_" + AppUserId);
                }
                else
                {
                    //if (LoginProvider == "Cookie")
                    //{
                    //    //user = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginUserKey).ToString()).ToObject<Operator>();
                    //    //#region 解决cookie时，设置数据权限较多时无法登陆的bug
                    //    //AuthorizeDataModel dataAuthorize = CacheFactory.Cache().GetCache<AuthorizeDataModel>(user.UserId + "_" + LoginUserKey);
                    //    //user.DataAuthorize = dataAuthorize;
                    //    //#endregion
                    //    //user = FormsAuth.GetUserData();

                    //}
                    //else
                    //{
                    //    user = CacheFactory.Cache().GetCache<Operator>(FormsAuth.GetUserKey());
                    //    //user = DESEncrypt.Decrypt(WebHelper.GetSession(LoginUserKey).ToString()).ToObject<Operator>();
                    //}
                    user = CacheFactory.Cache().GetCache<Operator>(FormsAuth.GetUserKey());
                    //if (user == null)
                    //{
                    //    //user = CacheFactory.Cache().GetCache<Operator>(systemName + "_" + OperatorProvider.AppUserId);

                    //}
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除登录信息
        /// </summary>
        public virtual void EmptyCurrent()
        {
            FormsAuth.SingOut();
            if (LoginProvider == "Cookie")
            {
                string val = WebHelper.GetCookie(LoginUserKey);
                if (!string.IsNullOrEmpty(val))
                {
                    Operator user = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginUserKey)).ToObject<Operator>();
                    WebHelper.RemoveCookie(LoginUserKey.Trim());
                    CacheFactory.Cache().RemoveCache(LoginUserKey);
                    if (user != null)
                    {
                        CacheFactory.Cache().RemoveCache(systemName + "_" + user.UserId + "_" + LoginUserKey);
                    }
                }

            }
            else
            {
                WebHelper.RemoveSession(LoginUserKey);
            }
        }
        /// <summary>
        /// 是否过期
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOverdue()
        {
            try
            {
                //object str = "";
                ////AuthorizeDataModel dataAuthorize = null;
                //if (LoginProvider == "Cookie")
                //{
                //    str = WebHelper.GetCookie(LoginUserKey);
                //    Operator user = DESEncrypt.Decrypt(str.ToString()).ToObject<Operator>();
                //    //dataAuthorize = CacheFactory.Cache().GetCache<AuthorizeDataModel>(user.UserId + "_" + LoginUserKey);
                //    //if (dataAuthorize==null)
                //    //{
                //    //    return true;
                //    //}
                //    //if (string.IsNullOrEmpty(WebHelper.GetCookie(FormsAuthentication.FormsCookieName)))
                //    //{
                //    //    return true;
                //    //}
                //    if (string.IsNullOrEmpty(WebHelper.GetCookie(LoginUserKey)))
                //    {
                //        return true;
                //    }
                //}
                //else
                //{
                //    str = WebHelper.GetSession(LoginUserKey);
                //}
                //if (str != null && str.ToString() != "")
                //{
                //    return false;
                //}
                //else
                //{
                //    return true;
                //}
                if (string.IsNullOrEmpty(WebHelper.GetCookie(Config.GetValue("SoftName"))))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return true;
            }
        }
        /// <summary>
        /// 是否已登录
        /// </summary>
        /// <returns></returns>
        public virtual int IsOnLine()
        {
            Operator user = new Operator();
            user = CacheFactory.Cache().GetCache<Operator>(FormsAuth.GetUserKey());
            if (user == null)
            {
                return -1;
            }
            //if (LoginProvider == "Cookie")
            //{
            //    //user = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginUserKey).ToString()).ToObject<Operator>();
            //     //AuthorizeDataModel dataAuthorize = CacheFactory.Cache().GetCache<AuthorizeDataModel>(user.UserId + "_" + LoginUserKey);
            //}
            //else
            //{
            //    user = DESEncrypt.Decrypt(WebHelper.GetSession(LoginUserKey).ToString()).ToObject<Operator>();
            //}
            object token = CacheFactory.Cache().GetCache<string>(systemName + "_" + user.UserId + "_" + LoginUserKey);
            if (token == null)
            {
                return -1;//过期
            }
            if (user.Token == token.ToString())
            {
                return 1;//正常
            }
            else
            {
                return 0;//已登录
            }
        }
    }
}
