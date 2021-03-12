using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace BSFrameWork.Application.AppInterface.App_Start
{
    public class UnhandleExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            NLog.LogManager.GetCurrentClassLogger().Error(actionExecutedContext.Exception);
            base.OnException(actionExecutedContext);
        }
    }
}