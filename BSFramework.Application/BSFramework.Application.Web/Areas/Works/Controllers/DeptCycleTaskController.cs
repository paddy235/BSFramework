using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.DeptCycleTaskManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.DeptCycleTaskManage;
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
    public class DeptCycleTaskController : MvcControllerBase
    {
        #region 页面
        /// <summary>
        ///周期任务库首页
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

            return View();
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
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
        /// <summary>
        /// 周期
        /// </summary>
        /// <returns></returns>
        public ActionResult Cycle()
        {
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
            var deptcycle = new DeptCycleTaskBLL();
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
            var deptcycle = new DeptCycleTaskBLL();
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
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(DeptCycleTaskEntity entity)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                var deptcycle = new DeptCycleTaskBLL();

                deptcycle.SaveForm(entity, user.UserId);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]

        public ActionResult delEntity(string keyvalue)
        {
            try
            {

                var deptcycle = new DeptCycleTaskBLL();

                deptcycle.deleteEntity(keyvalue);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        #region 导入
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoImport()
        {
            var success = true;
            var message = "保存成功！";
            var user = OperatorProvider.Provider.Current();

            if (this.Request.Files.Count > 0)
            {
                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];
                var bll = new DeptCycleTaskBLL();
                try
                {
                    var titleIndex = this.GetTitleRow(sheet);
                    var data = this.GetData(sheet, titleIndex, user.UserId);
                    bll.SaveForm(data, user.UserId);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }
        private List<DeptCycleTaskEntity> GetData(Worksheet sheet, int titleIndex, string userid)
        {
            var userbll = new UserBLL();
            var user = userbll.GetEntity(userid);
            var total = 0;
            var deptuser = userbll.GetList(user.DepartmentId, 10000, 1, out total);
            var result = new List<DeptCycleTaskEntity>();
            for (int i = titleIndex + 1; i <= sheet.Cells.MaxDataRow; i++)

            {
                var entity = new DeptCycleTaskEntity()
                {
                    //id = Guid.NewGuid().ToString(),
                    content = sheet.Cells[i, 0].StringValue
                };
                if (string.IsNullOrEmpty(entity.content) && string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue)) break;
                if (string.IsNullOrEmpty(entity.content)) throw new Exception(string.Format("行 {0} 任务内容为空！", i + 1));
                var workuser = sheet.Cells[i, 2].StringValue;
                var ckworkuser = deptuser.FirstOrDefault(x => x.RealName == workuser);
                if (ckworkuser == null)
                {
                    throw new Exception(string.Format("行 {0} 部门不存在" + workuser + "责任人！", i + 1));
                }
                else
                {
                    entity.dutyuser = ckworkuser.RealName;
                    entity.dutyuserid = ckworkuser.UserId;
                }
                if (!string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue))
                {
                    //每年，二月、九月，15日，白班  每月，第一个、第三个，星期五，白班
                    var Cycle = sheet.Cells[i, 1].StringValue.Trim();

                    var cycleType = Cycle.Split('，');
                    if (cycleType[0] != "每天" && cycleType[0] != "每周" && cycleType[0] != "每月" && cycleType[0] != "每年")
                    {
                        throw new Exception(string.Format("行 {0} 周期规则错误！", i + 1));
                    }

                    entity.cycle = cycleType[0];
                    var ck = false;
                    var data = string.Empty;
                    for (int j = 1; j < cycleType.Length; j++)
                    {

                        //if (cycleType[j] == "白班" || cycleType[j] == "夜班")
                        //{
                        //    entity.worksetname = cycleType[j];

                        //}
                        //else
                        if (cycleType[j] == "截止")
                        {
                            entity.isend = true;

                        }
                        else
                        if (cycleType[j] == "最后一天")
                        {
                            if (Cycle.Contains("每月") || Cycle.Contains("每年"))
                            {
                                entity.islastday = true;
                            }
                            else
                            {
                                throw new Exception(string.Format("行 {0} 周期规则错误！", i + 1));

                            }

                        }
                        else
                        if (cycleType[j].Contains("双休"))
                        {
                            if (Cycle.Contains("每月") || Cycle.Contains("每年"))
                            {
                                entity.isweek = true;
                            }
                            else if (Cycle.Contains("每天"))
                            {
                                entity.isweek = true;
                            }
                            else
                            {
                                throw new Exception(string.Format("行 {0} 周期规则错误！", i + 1));

                            }
                        }
                        else
                        {
                            data += cycleType[j].Replace('日', ' ').Trim().Replace('、', ',') + ";";
                            ck = true;
                        }


                    }

                    if (ck)
                    {
                        data = data.Substring(0, data.Length - 1);
                        entity.cycledate = data;
                    }
                    else
                    {
                        entity.cycledate = data;
                    }

                }
                else
                {
                    throw new Exception(string.Format("行 {0} 周期不能为空！", i + 1));
                }
                result.Add(entity);
            }

            return result;
        }
        private int GetTitleRow(Worksheet sheet)
        {
            for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
            {
                if (sheet.Cells[i, 0].StringValue == "工作任务") return i;
            }

            throw new Exception("无法识别文件！");
        }
        #endregion


        #endregion
    }
}