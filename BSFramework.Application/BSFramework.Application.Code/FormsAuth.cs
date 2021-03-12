using BSFramework.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace BSFramework.Application.Code
{
    public class FormsAuth
    {
        public static void SignIn(string loginName, Operator userData, int expireMin)
        {

            //string data = JsonConvert.SerializeObject(new
            //{
            //    Code = userData.Code,
            //    DeptCode = userData.DeptCode,
            //    OrganizeCode = userData.OrganizeCode,
            //    OrganizeId = userData.OrganizeId,
            //    DeptId = userData.DeptId,
            //    RoleName = userData.RoleName,
            //    PostName = userData.PostName,
            //    DeptName = userData.DeptName,
            //    OrganizeName = userData.OrganizeName,
            //    Secretkey = userData.Secretkey,
            //    DutyName = userData.DutyName,
            //    Password = userData.Password,
            //    Photo = userData.Photo,
            //    IsSystem = userData.IsSystem,
            //    UserId = userData.UserId,
            //    Token = userData.Token,
            //    Account = userData.Account,
            //    UserName = userData.UserName,
            //    ObjectId = userData.ObjectId
            //});
            ////创建一个FormsAuthenticationTicket，它包含登录名以及额外的用户数据。
            //var ticket = new FormsAuthenticationTicket(2,
            //    loginName, DateTime.Now, DateTime.Now.AddDays(1), true, loginName);

            ////加密Ticket，变成一个加密的字符串。
            //var cookieValue = FormsAuthentication.Encrypt(ticket);

            ////根据加密结果创建登录Cookie
            //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
            //{
            //    HttpOnly = true,
            //    Domain = FormsAuthentication.CookieDomain,
            //    Path = FormsAuthentication.FormsCookiePath
            //};
            //if (expireMin > 0)
            //    cookie.Expires = DateTime.Now.AddMinutes(expireMin);

            //var context = HttpContext.Current;
            //if (context == null)
            //    throw new InvalidOperationException();

            ////写登录Cookie
            //context.Response.Cookies.Remove(cookie.Name);
            //context.Response.Cookies.Add(cookie);
            WebHelper.WriteCookie(Config.GetValue("SoftName"), BSFramework.Util.DESEncrypt.Encrypt(loginName));
        }

        public static void SingOut()
        {
            HttpContext.Current.Session.Clear();
            FormsAuthentication.SignOut();
        }

      
        public static string GetUserKey()
        {
            var context = HttpContext.Current;
            var cookie = context.Request.Cookies[Config.GetValue("SoftName")];
            if (cookie != null)
            {
                return BSFramework.Util.DESEncrypt.Decrypt(cookie.Value);
            }
            else
            {
                return "";
            }
        }
    }
}
