using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.DeptCycleTaskManage;
using BSFramework.Application.Code;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{

    /// <summary>
    /// 部门周期任务
    /// </summary>
    public class DeptWorkCycleTaskController : MvcControllerBase
    {
        #region 页面
        /// <summary>
        ///周期任务
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();

            var userbll = new UserBLL();
            var userlist = userbll.GetList();
            if (string.IsNullOrEmpty(user.DeptCode))
            {
                ViewData["deptuser"] = userlist;

            }
            else
            {
                //ViewData["deptuser"] = userbll.GetDeptUsers(user.DeptId);
                ViewData["deptuser"] = userlist.Where(x => !string.IsNullOrEmpty(x.DepartmentCode)).Where(x => x.DepartmentId == user.DeptId).ToList();
            }
            ViewBag.rolename = user.RoleName;
            ViewBag.userid = user.UserId;
            return View();
        }

        /// <summary>
        ///修改 新增
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.dept = user.DeptId;
            ViewBag.user = user.UserName;
            return View();

        }

        #endregion

        #region 获取
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //var queryParam = queryJson.ToJObject();
            var deptcycle = new DeptWorkCycleTaskBLL();
            var user = OperatorProvider.Provider.Current();
            var data = deptcycle.GetPageList(pagination, queryJson, user.UserId);
            foreach (var item in data)
            {
                item.cycleDataStr = item.cycle + " " + item.cycledate;
                if (item.islastday)
                {
                    item.cycleDataStr += " " + "最后一天";
                }
                if (item.isweek)
                {
                    item.cycleDataStr += " " + "跳过双休";
                }
                if (item.isend)
                {
                    item.cycleDataStr += " " + "截止";
                }
            }
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult getEntity(string keyvalue)
        {
            var deptcycle = new DeptWorkCycleTaskBLL();
            var data = deptcycle.getEntity(keyvalue);
            data.cycleDataStr = data.cycle + " " + data.cycledate;
            if (data.islastday)
            {
                data.cycleDataStr += " " + "最后一天";
            }
            if (data.isweek)
            {
                data.cycleDataStr += " " + "跳过双休";
            }
            if (data.isend)
            {
                data.cycleDataStr += " " + "截止";
            }
            return Content(data.ToJson());
        }
        #endregion

        #region 操作
        /// <summary>
        /// 状态
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult PostState(string keyvalue, string state)
        {
            var deptcycle = new DeptWorkCycleTaskBLL();
            var user = OperatorProvider.Provider.Current();
            try
            {
                var data = deptcycle.getEntity(keyvalue);
                if (state == "true")
                {
                    data.workstate = "已完成";
                }
                if (state == "false")
                {
                    data.workstate = "进行中";
                }
                if (state == "取消")
                {
                    data.workstate = "已取消";
                }
                deptcycle.SaveForm(data, user.UserId);
                return Success("操作成功");
            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }

        #endregion
    }
}