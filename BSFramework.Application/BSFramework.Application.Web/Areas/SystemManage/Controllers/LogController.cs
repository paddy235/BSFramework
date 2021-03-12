using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 描 述：系统日志
    /// </summary>
    public class LogController : MvcControllerBase
    {
        #region 视图功能
        /// <summary>
        /// 日志管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RemoveLog()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public JsonResult GetPageListJson(int? categoryid, string starttime, string endtime, string operateaccount, string ipaddress, string operatetype, string module, int rows, int page)
        {
            var from = DateTime.Parse(starttime);
            var to = DateTime.Parse(endtime).AddDays(1).AddSeconds(-1);
            var total = 0;
            var data = LogBLL.GetPageList(categoryid, from, to, operateaccount, ipaddress, operatetype, module, rows, page, out total);
            return Json(new { rows = data, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);


            //pagination.p_kid = "LOGID";
            //pagination.p_fields = "OperateTime,OperateAccount,IPAddress,Module,OperateType,ExecuteResult,ExecuteResultJson,CategoryId";
            //pagination.p_tablename = "BASE_LOG t";
            //pagination.conditionJson = " 1=1"; 
            //var watch = CommonHelper.TimerStart();
            //var data = LogBLL.GetPageList(pagination, queryJson);
            //var JsonData = new
            //{
            //    rows = data,
            //    total = pagination.total,
            //    page = pagination.page,
            //    records = pagination.records,
            //    costtime = CommonHelper.TimerEnd(watch)
            //};
            //return Content(JsonData.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 清空日志
        /// </summary>
        /// <param name="categoryId">日志分类Id</param>
        /// <param name="keepTime">保留时间段内</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "清空系统日志")]
        public ActionResult RemoveLog(int categoryId, string keepTime)
        {
            LogBLL.RemoveLog(categoryId, keepTime);
            return Success("清空成功。");
        }
        #endregion
    }
}
