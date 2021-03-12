using Aspose.Cells;
using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.WorkMeeting;
using BSFramework.Application.Web.Areas.BaseManage.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 人身风险预控
    /// </summary>
    public class DangerMeasureController : MvcControllerBase
    {
        public ViewResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var config = Config.GetValue("TrainingDept");
            // var isallowd = user.DeptName == config || user.IsSystem;
            var isallowd = user.DeptName == config || user.RoleName.Contains("公司管理员") || user.RoleName.Contains("厂级管理员") || user.IsSystem;//人身风险预控库和设置，应只有公司管理员和安监部(HSE部)可操作，其他人只能查看 hm 2019-06-28
            ViewBag.isallowd = isallowd;
            return View();
        }

        public JsonResult GetCategories(string categoryid)
        {
            var bll = new DangerMeasureBLL();
            var data = bll.GetCategories(categoryid).OrderBy(x => x.Sort).ToList();
            return Json(data.Where(x => x.ParentCategoryId == null).Select(x => new TreeModel { id = x.CategoryId.ToString(), value = x.CategoryId.ToString(), text = x.CategoryName, isexpand = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, hasChildren = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, ChildNodes = GetChildren(data, x.CategoryId) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ViewResult Edit(string id)
        {
            var model = default(DangerCategoryEntity);
            if (string.IsNullOrEmpty(id))
            {
                model = new DangerCategoryEntity();
            }
            else
            {
                var guid = Guid.Parse(id);
                var bll = new DangerMeasureBLL();
                model = bll.GetCategory(guid);
            }
            return View(model);
        }

        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(string id, DangerCategoryEntity model)
        {
            var bll = new DangerMeasureBLL();

            var success = true;
            var message = "保存成功";

            try
            {
                bll.Save(new DangerCategoryEntity() { CategoryName = model.CategoryName, CategoryId = id, Sort = model.Sort });
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 风险预控措施
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Edit2(string id)
        {
            DangerMeasureEntity model = null;
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new DangerMeasureBLL();
                model = bll.GetMeasureDetail(id);
            }
            else
                model = new DangerMeasureEntity();

            return View(model);
        }

        /// <summary>
        /// 新增和修改风险预控措施
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit2(string id, DangerMeasureEntity model)
        {
            var bll = new DangerMeasureBLL();
            var user = OperatorProvider.Provider.Current();

            var success = true;
            var message = "保存成功";
            try
            {
                model.OperateUserId = user.UserId;
                model.OperateUser = user.UserName;
                model.OperateTime = DateTime.Now;
                bll.SaveMeasure(model);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {
            var bll = new DangerMeasureBLL();

            var success = true;
            var message = "删除成功";

            try
            {
                bll.Delete(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Remove(string id)
        {
            var bll = new DangerMeasureBLL();

            var success = true;
            var message = "删除成功";

            try
            {
                bll.DeleteMeasure(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ActionResult Import()
        {
            return View();
        }

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

                var bll = new DangerMeasureBLL();
                try
                {
                    var titleIndex = this.GetTitleRow(sheet);

                    var measuredata = this.GetMeasureData(sheet, titleIndex);
                    foreach (var item in measuredata)
                    {
                        item.OperateTime = DateTime.Now;
                        item.OperateUser = user.UserName;
                        item.OperateUserId = user.UserId;
                    }
                    bll.AddMeasures(measuredata);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        public JsonResult GetDangerReasons(string categoryid)
        {
            var bll = new DangerMeasureBLL();
            var data = bll.GetDangerReasons(categoryid);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private int GetTitleRow(Worksheet sheet)
        {
            for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
            {
                if (sheet.Cells[i, 0].StringValue == "风险类别") return i;
            }

            throw new Exception("无法识别文件！");
        }

        private List<DangerMeasureEntity> GetMeasureData(Worksheet sheet, int titleIndex)
        {
            var result = new List<DangerMeasureEntity>();
            var bll = new DangerMeasureBLL();
            var categories = bll.GetCategories(null);

            for (int i = titleIndex + 1; i <= sheet.Cells.MaxDataRow; i++)
            {
                var measure = new DangerMeasureEntity()
                {
                    MeasureId = Guid.NewGuid().ToString(),
                    Category = sheet.Cells[i, 0].StringValue,
                    DangerReason = sheet.Cells[i, 1].StringValue,
                    MeasureContent = sheet.Cells[i, 2].StringValue
                };
                if (string.IsNullOrEmpty(measure.DangerReason)) throw new Exception(string.Format("行 {0} 风险因素为空！", i + 1));
                var category = categories.FirstOrDefault(x => x.CategoryName == measure.Category);
                if (category == null) throw new Exception(string.Format("行 {0} 风险类别不存在！", i + 1));
                else measure.CategoryId = category.CategoryId;

                if (bll.ExistDangerReason(measure.CategoryId, measure.DangerReason)) throw new Exception(string.Format("行 {0} 风险因素已存在！", i + 1));

                if (result.Count(x => x.CategoryId == measure.CategoryId && x.DangerReason == measure.DangerReason) > 0) throw new Exception(string.Format("行 {0} 风险因素重复！", i + 1));
                result.Add(measure);
            }

            return result;
        }

        public JsonResult GetData(FormCollection fc)
        {
            var categoryid = fc.Get("categoryid");
            var key = fc.Get("key");
            var page = int.Parse(fc.Get("page") ?? "1");
            var rows = int.Parse(fc.Get("rows") ?? "20");
            var sortfield = fc.Get("sidx");
            var direction = fc.Get("sord");
            var bll = new DangerMeasureBLL();
            var user = OperatorProvider.Provider.Current();
            var total = 0;
            var data = bll.GetData(categoryid, key, rows, page, out total, sortfield, direction);


            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page });
        }

        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<DangerCategoryEntity> data, string id)
        {
            return data.Where(x => x.ParentCategoryId == id).Select(x => new TreeModel { id = x.CategoryId.ToString(), value = x.CategoryId.ToString(), text = x.CategoryName, isexpand = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, hasChildren = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, ChildNodes = GetChildren(data, x.CategoryId) }).ToList();
        }

        /// <summary>
        /// 设置风险预控规则
        /// </summary>
        /// <returns></returns>
        public ActionResult SetRole()
        {
            var bll = new DataItemDetailBLL();
            DataItemDetailEntity entity = bll.GetEntity("szfxykgz");
            return View(entity);
        }

        [HttpPost]
        public ActionResult SaveRole(DataItemDetailEntity entity)
        {
            try
            {
                var bll = new DataItemDetailBLL();
                bll.SaveOrUpdateRole("szfxykgz", entity);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error("操作失败：" + ex.Message);
            }

        }


        public ViewResult Setting()
        {
            var settings = new DataItemDetailBLL().GetDataItems("人身风险预控");
            ViewBag.settings = settings;
            return View();
        }

        public JsonResult SaveSetting(DataItemDetailEntity model)
        {
            var success = true;
            var message = "保存成功";
            try
            {
                var setting = new DataItemDetailBLL().GetDetail("人身风险预控", "人身风险预控关联工作任务状态");
                var bll = new DataItemDetailBLL();
                setting.ItemValue = model.ItemValue;
                bll.SaveForm(setting.ItemDetailId, setting);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        [HttpPost]
        public JsonResult EditSetting(DataItemDetailEntity[] models)
        {
            var success = true;
            var message = "保存成功";

            var bll = new DataItemDetailBLL();
            try
            {
                bll.EditList("人身风险预控", models);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }
    }
}
