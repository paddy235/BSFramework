using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SetManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.SetManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SetManage.Controllers
{
    /// <summary>
    /// 描 述：区域管理
    /// </summary>
    public class RiskFactorSetController : MvcControllerBase
    {
        private RiskFactorSetBLL riskFactorSetBLL = new RiskFactorSetBLL();
        private MeasureSetBLL measureSetBLL = new MeasureSetBLL();

        /// <summary>
        /// 危险因素设置页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptcode = dept.EnCode;
            ViewBag.deptname = user.DeptName;
            ViewBag.userid = user.UserId;
            return View();
        }

        public ViewResult Edit(string id)
        {
            var model = riskFactorSetBLL.GetEntity(id);
            model.measures = riskFactorSetBLL.GetList(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, bool isCreate, RiskFactorSetEntity model)
        {
            if (isCreate)
            {
                if (model.measures == null)
                    model.measures = new List<MeasureSetEntity>();

                model.measures.Add(new MeasureSetEntity());

                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            var currentUser = OperatorProvider.Provider.Current();
            model.ModifyDate = model.CreateDate = DateTime.Now;
            model.CreateDeptCode = currentUser.DeptCode;
            model.CreateDeptId = currentUser.DeptId;
            model.CreateUserName = model.ModifyUserName = currentUser.UserName;
            model.CreateUserId = model.ModifyUserId = currentUser.UserId;
            model.DeptId = currentUser.DeptId;
            model.DeptName = currentUser.DeptName;
            foreach (var item in model.measures)
            {
                item.CreateDate = DateTime.Now;
                item.RiskFactorId = model.ID;
            }
            riskFactorSetBLL.SaveForm(model);

            ViewBag.callback = "jQuery(saveCallback);";
            ViewBag.id = id;

            return View(model);
        }

        public ViewResult Create()
        {
            var model = new RiskFactorSetEntity() { measures = new List<MeasureSetEntity> { new MeasureSetEntity() } };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(bool isCreate, RiskFactorSetEntity model)
        {
            if (isCreate)
            {
                if (model.measures == null)
                    model.measures = new List<MeasureSetEntity>();

                model.measures.Add(new MeasureSetEntity());

                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            if (model.measures == null || model.measures.Count == 0)
            {
                model.measures = new List<MeasureSetEntity>();

                ModelState.AddModelError("measures", "没有防范措施");
                return View(model);
            }

            var currentUser = OperatorProvider.Provider.Current();
            model.ID = Guid.NewGuid().ToString();
            model.ModifyDate = model.CreateDate = DateTime.Now;
            model.CreateDeptCode = currentUser.DeptCode;
            model.CreateDeptId = currentUser.DeptId;
            model.CreateUserName = model.ModifyUserName = currentUser.UserName;
            model.CreateUserId = model.ModifyUserId = currentUser.UserId;
            model.DeptId = currentUser.DeptId;
            model.DeptName = currentUser.DeptName;
            foreach (var item in model.measures)
            {
                item.ID = Guid.NewGuid().ToString();
                item.CreateDate = DateTime.Now;
                item.RiskFactorId = model.ID;
            }
            riskFactorSetBLL.SaveForm(model);

            ViewBag.callback = "jQuery(saveCallback);";
            ViewBag.id = model.ID;

            return View(model);
        }

        public ActionResult Form(string keyValue, string deptid, string deptcode)
        {
            ViewBag.rootdeptid = new DepartmentBLL().GetRootDepartment().DepartmentId;
            ViewBag.keyValue = keyValue;
            ViewBag.deptid = deptid;
            ViewBag.deptcode = deptcode;
            RiskFactorSetEntity riskFactorSetEntity = null;
            if (!string.IsNullOrEmpty(keyValue))
            {
                //危险因素
                riskFactorSetEntity = riskFactorSetBLL.GetEntity(keyValue);
                //防范措施
                riskFactorSetEntity.measures = measureSetBLL.GetList(riskFactorSetEntity.ID).ToList();
            }
            else
            {
                if (!string.IsNullOrEmpty(deptid))
                {
                    var dept = new DepartmentBLL().GetEntity(deptid);
                    riskFactorSetEntity = new RiskFactorSetEntity();
                    riskFactorSetEntity.DeptId = dept?.DepartmentId;
                    riskFactorSetEntity.DeptName = dept?.FullName;
                }
            }
            return View(riskFactorSetEntity);
        }

        public ActionResult Import(string deptid, string deptcode)
        {
            ViewBag.deptid = deptid;
            ViewBag.deptcode = deptcode;

            return View();
        }

        #region 获取数据
        /// <summary>
        /// 获取分页数据 
        /// </summary>
        public ActionResult GetPageList(Pagination pagination, string queryJson)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                pagination.conditionJson = " 1=1 ";

                var data = riskFactorSetBLL.GetPageList(pagination, queryJson);
                var jsonData = new
                {
                    rows = data,
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(jsonData);
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public JsonResult List()
        {
            var currentUser = OperatorProvider.Provider.Current();
            var list = riskFactorSetBLL.GetList(currentUser.DeptId, null);
            foreach (var item in list)
            {
                item.measures = measureSetBLL.GetList(item.ID).ToList();
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除危险因素
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "删除危险因素")]
        public ActionResult RemoveForm(string keyValue)
        {
            riskFactorSetBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }

        /// <summary>
        /// 保存危险因素表单（新增、修改）
        /// </summary>
        /// <param name="jsonData">实体JSON数据</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存危险因素表单(新增、修改)")]
        public ActionResult SaveForm(string jsonData)
        {
            RiskFactorSetEntity entity = JsonConvert.DeserializeAnonymousType(jsonData, new RiskFactorSetEntity());
            riskFactorSetBLL.SaveForm(entity);
            return Success("操作成功。");
        }

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
            string deptid = this.Request["deptid"];
            string deptcode = this.Request["deptcode"];
            if (this.Request.Files.Count > 0)
            {
                try
                {
                    var book = new Workbook(this.Request.Files[0].InputStream);
                    var sheet = book.Worksheets[0];
                    var result = new List<RiskFactorSetEntity>();
                    var items = new List<RiskFactorSetEntity>();

                    for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                    {
                        var riskFactorSetEntity = new RiskFactorSetEntity();
                        riskFactorSetEntity.Content = sheet.Cells[i, 0].StringValue;
                        if (string.IsNullOrEmpty(riskFactorSetEntity.Content.Trim()))
                        {
                            throw new ArgumentException("第" + (i + 1) + "行，危险因素不能为空");
                        }
                        //riskFactorSetEntity.DeptId = deptid;
                        riskFactorSetEntity.measures = new List<MeasureSetEntity>();
                        for (int j = 1; j <= sheet.Cells.MaxDataColumn; j++)
                        {
                            if (!string.IsNullOrEmpty(sheet.Cells[i, j].StringValue))
                            {
                                var measure = new MeasureSetEntity()
                                {
                                    Content = sheet.Cells[i, j].StringValue
                                };
                                riskFactorSetEntity.measures.Add(measure);
                            }
                        }
                        result.Add(riskFactorSetEntity);

                        int measuresNum = riskFactorSetEntity.measures.Where(x => x.Content.Trim().Length > 0).Count();
                        if (measuresNum < 1)
                        {
                            throw new ArgumentException("第" + (i + 1) + "行，防范措施至少填写一项");
                        }
                    }

                    var ary = deptid.Split(',');
                    foreach (var item in ary)
                    {
                        foreach (var item1 in result)
                        {
                            var rk = new RiskFactorSetEntity()
                            {
                                Content = item1.Content,
                                DeptId = item,
                                measures = new List<MeasureSetEntity>()
                            };
                            foreach (var item2 in item1.measures)
                            {
                                rk.measures.Add(new MeasureSetEntity() { Content = item2.Content });
                            }
                            items.Add(rk);
                        }
                    }

                    foreach (var item in items)
                    {
                        riskFactorSetBLL.SaveForm(item);
                    }
                }
                catch (CellsException e)
                {
                    success = false;
                    message = "请选择正确的导入文件";
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }
        #endregion
    }
}
