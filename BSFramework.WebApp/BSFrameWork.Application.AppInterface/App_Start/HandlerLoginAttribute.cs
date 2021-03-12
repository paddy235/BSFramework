using System.Web.Mvc;
using BSFramework.Application.Code;
using BSFramework.Util;
using System.Web;

namespace BSFrameWork.Application.AppInterface
{
    /// <summary>
    /// 描 述：登录认证（会话验证组件）
    /// </summary>
    public class HandlerLoginAttribute : AuthorizeAttribute
    {
        private LoginMode _customMode;
        /// <summary>默认构造</summary>
        /// <param name="Mode">认证模式</param>
        public HandlerLoginAttribute(LoginMode Mode)
        {
            _customMode = Mode;
        }
        /// <summary>
        /// 响应前执行登录验证,查看当前用户是否有效 
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //登录拦截是否忽略
            if (_customMode == LoginMode.Ignore)
            {
                return;
            }
            //身份票据是否过期
            if (OperatorProvider.Provider.IsOverdue())
            {
                WebHelper.WriteCookie("login_error", "Overdue");//登录已超时,请重新登录
                filterContext.Result = new RedirectResult("~/Login/Index");
                return;
            }
            //是否已登录
            var OnLine = OperatorProvider.Provider.IsOnLine();
            if (OnLine == 0)
            {
                //是否允许同一账号多个地方登录
               if (Config.GetValue("CheckOnLine") == "false")
               {
                   WebHelper.WriteCookie("login_error", "OnLine");//您的帐号已在其它地方登录,请重新登录
                   filterContext.Result = new RedirectResult("~/Login/Index");
               }
                return;
            }
            else if (OnLine == -1)
            {
                WebHelper.WriteCookie("login_error", "-1");//缓存已超时,请重新登录
                //filterContext.Result = new RedirectResult("~/Login/Default");
                return;
            }
        }
    }
}