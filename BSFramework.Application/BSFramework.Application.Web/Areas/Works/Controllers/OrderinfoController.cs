using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Busines.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    public class OrderinfoController : MvcControllerBase
    {
        private OrderinfoBLL orderinfobll = new OrderinfoBLL();

        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            Operator user = OperatorProvider.Provider.Current();
            var number = orderinfobll.GetMakeList(user.UserId, user.OrganizeCode).Count();
            ViewBag.number = number;
            ViewBag.username = OperatorProvider.Provider.Current().UserName;
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpPost]
        public string GetListJson(string queryJson)
        {
            Operator user = OperatorProvider.Provider.Current();
            var data = orderinfobll.GetList(user.OrganizeCode);
            return data == null ? Newtonsoft.Json.JsonConvert.SerializeObject(new { list = new List<string>() }) : Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }
        /// <summary>
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = orderinfobll.GetEntity(keyValue);

            return ToJsonResult(new { formData = data });
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string keyValue)
        {
            orderinfobll.RemoveForm(keyValue);
            return Success("删除成功。");
        }

        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="groupId">班组Id</param>
        /// <param name="sId">主表记录Id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string groupId, string sId)
        {
            string userName = OperatorProvider.Provider.Current().UserName;
            string res = orderinfobll.SaveForm(groupId, sId, userName);
            if (res == "-1")
            {
                return Error("已预约");
            }
            else
            {
                return res.Length > 0 ? Success("预约成功。", res) : Error("预约失败");
            }

        }
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail()
        {
            return View();
        }

        public JsonResult GetData(int rows)
        {
            var page = int.Parse(this.Request.QueryString.Get("page") ?? "1");
            var pagesize = int.Parse(this.Request.QueryString.Get("rows") ?? "20");
            var total = 0;

            var user = OperatorProvider.Provider.Current();
            //var data = bll.GetEvaluationsManoeuvre(name, rows, page, ToCompileDeptIdSearch, EmergencyTypeSearch, meetingstarttime, meetingendtime, out total);
            var data = orderinfobll.GetDetailData(user.UserId, user.UserName, rows, page, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}
