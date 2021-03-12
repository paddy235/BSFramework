using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.ProducingCheck;
using BSFramework.Application.Web.Areas.ProducingCheck.Models;
using BSFramework.Application.Web.Areas.SystemManage.Models;
using BSFramework.Busines.ProducingCheck;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.ProducingCheck.Controllers
{
    /// <summary>
    /// 安全文明生产检查问题库
    /// </summary>
    public class CheckTemplateController : MvcControllerBase
    {
        private CheckTemplateBLL checkTemplateBLL = new CheckTemplateBLL();
        private CheckCategoryBLL checkCategoryBLL = new CheckCategoryBLL();


        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        public ViewResult Index()
        {
            return View();
        }

        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult Edit(string id, string categoryId)
        {
            var user = OperatorProvider.Provider.Current();
            var model = default(TemplateModel);

            if (string.IsNullOrEmpty(id))
            {
                var category = checkCategoryBLL.Get(categoryId);
                model = new TemplateModel
                {
                    CategoryId = category?.CategoryId,
                    CategoryName = category?.CategoryName,
                    OperateDate = DateTime.Now,
                    OperateUser = user.UserName
                };
            }
            else
            {
                var data = checkTemplateBLL.Get(id);
                model = new TemplateModel
                {
                    TemplateId = data.TemplateId,
                    CategoryId = data.CategoryId,
                    CategoryName = data.CategoryName,
                    DistrictId = data.DistrictId,
                    DistrictName = data.DistrictName,
                    DutyDepartmentId = data.DutyDepartmentId,
                    DutyDepartmentName = data.DutyDepartmentName,
                    ProblemContent = data.ProblemContent,
                    ProblemMeasure = data.ProblemMeasure,
                    OperateDate = DateTime.Now,
                    OperateUser = user.UserName
                };
            }

            return View(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(string id, TemplateModel model)
        {
            var success = true;
            var message = "保存成功";

            var user = OperatorProvider.Provider.Current();

            var template = new CheckTemplateEntity
            {
                TemplateId = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id,
                CategoryId = model.CategoryId,
                CategoryName = model.CategoryName,
                ProblemContent = model.ProblemContent,
                ProblemMeasure = model.ProblemMeasure,
                DutyDepartmentId = model.DutyDepartmentId,
                DutyDepartmentName = model.DutyDepartmentName,
                DistrictId = model.DistrictId,
                DistrictName = model.DistrictName,
                CreateDate = model.OperateDate,
                CreateUserId = user.UserId,
                ModifyDate = model.OperateDate,
                ModifyUserId = user.UserId
            };

            try
            {
                checkTemplateBLL.Edit(template);
            }
            catch (Exception e)
            {
                success = false;
                message = e.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult List(int rows, int page, string key, string categoryid)
        {
            var total = 0;
            var data = checkTemplateBLL.Get(categoryid, null, key, rows, page, out total);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {
            var success = true;
            var message = "删除成功";

            try
            {
                checkTemplateBLL.Delete(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
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

            var company = new DepartmentBLL().GetCompany(user.DeptId);

            var baseUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();

            var param1 = new { Data = new { companyId = company.DepartmentId } };
            var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param1));
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var json = client.PostAsync($"{baseUrl}District/GetDistrict", requestContent).Result.Content.ReadAsStringAsync().Result;
            var districts = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DistrictModel>>(json);

            var departments = new DepartmentBLL().GetAll();
            var userbll = new UserBLL();

            var categories = checkCategoryBLL.GetCategories();

            if (this.Request.Files.Count > 0)
            {
                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];

                try
                {
                    var data = new List<CheckTemplateEntity>();
                    for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[i, 0].StringValue) && string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue) && string.IsNullOrEmpty(sheet.Cells[i, 4].StringValue))
                            continue;

                        var item = new CheckTemplateEntity();
                        item.CategoryName = sheet.Cells[i, 0].StringValue;
                        item.ProblemContent = sheet.Cells[i, 1].StringValue;
                        item.ProblemMeasure = sheet.Cells[i, 2].StringValue;
                        item.DutyDepartmentName = sheet.Cells[i, 3].StringValue;
                        item.DistrictName = sheet.Cells[i, 4].StringValue;

                        //验证
                        if (string.IsNullOrEmpty(item.CategoryName)) throw new Exception($"行 {i + 1} 问题类别不能为空！");
                        if (string.IsNullOrEmpty(item.ProblemContent)) throw new Exception($"行 {i + 1} 问题描述不能为空！");
                        if (string.IsNullOrEmpty(item.DistrictName)) throw new Exception($"行 {i + 1} 区域范围不能为空！");

                        var category = categories.Find(x => x.CategoryName == item.CategoryName);
                        if (category == null) throw new Exception($"行 {i + 1} 问题类别不存在！");
                        else
                            item.CategoryId = category.CategoryId;

                        var district = districts.Find(x => x.DistrictName == item.DistrictName);
                        if (district == null) throw new Exception($"行 {i + 1} 区域名称不存在！");
                        else
                            item.DistrictId = district.DistrictID;

                        var department = departments.Find(x => x.FullName == item.DutyDepartmentName);
                        if (department != null)
                            item.DutyDepartmentId = department.DepartmentId;

                        item.CreateDate = DateTime.Now;
                        item.CreateUserId = item.ModifyUserId = user.UserId;
                        item.TemplateId = Guid.NewGuid().ToString();

                        data.Add(item);
                    }

                    checkTemplateBLL.Save(data);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }
            }
            else
            {
                success = false;
                message = "请选择导入文件！";
            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

    }
}
