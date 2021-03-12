using BSFramework.Application.Code;
using BSFramework.Util;
using BSFramework.Util.Log;
using BSFramework.Util.WebControl;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace BSFramework.Application.Web
{
    /// <summary>
    /// 描 述：控制器基类
    /// </summary>
    [HandlerLogin(LoginMode.Enforce)]
    public abstract class MvcControllerBase : Controller
    {
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            var Request = requestContext.HttpContext.Request;
            if (Request["mid"] != null)
            {
                if (!string.IsNullOrEmpty(Request["mid"]))
                {
                    HttpContext.Response.Cookies.Add(new System.Web.HttpCookie("currentmoduleId", Request["mid"].ToString()));
                    HttpContext.Response.Cookies.Add(new System.Web.HttpCookie("urlhost", HttpContext.Request.ApplicationPath));
                }
            }
        }

        private Log _logger;
        /// <summary>
        /// 日志操作
        /// </summary>
        public Log Logger
        {
            get { return _logger ?? (_logger = LogFactory.GetLogger(this.GetType().ToString())); }
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual ActionResult ToJsonResult(object data)
        {
            return Content(data.ToJson());
        }
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual ActionResult Success(string message)
        {
            return Content(new AjaxResult { type = ResultType.success, message = message }.ToJson());
        }

        protected virtual JsonResult Success()
        {
            return Json(new AjaxResult { type = ResultType.success });
        }

        protected virtual JsonResult Success(AjaxResult ajaxResult)
        {
            return Json(ajaxResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual ActionResult Success(string message, object data)
        {
            return Content(new AjaxResult { type = ResultType.success, message = message, resultdata = data }.ToJson());
        }
        /// <summary>
        /// 返回失败消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns></returns>
        protected virtual ActionResult Error(string message)
        {
            return Content(new AjaxResult { type = ResultType.error, message = message }.ToJson());
            //return View();
        }

        /// <summary>
        /// 跟据用户ID，模块ID，获取当前用户此模块的按钮和数据权限
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <param name="moduleid">模块ID</param>
        protected void GetPermission(string userid, string moduleid)
        {

        }
    }
}
