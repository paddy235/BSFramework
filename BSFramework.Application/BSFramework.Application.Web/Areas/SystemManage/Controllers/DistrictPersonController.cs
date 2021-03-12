using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Web.Areas.SystemManage.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 描 述：区域责任人
    /// </summary>
    public class DistrictPersonController : MvcControllerBase
    {
        private int idx = 0;
        private DistrictPersonBLL districtPersonBLL = new DistrictPersonBLL();
        public ViewResult Index()
        {
            return View();
        }

        public ViewResult Edit(string id)
        {
            var baseUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();
            var param = new { Data = "区域责任人设置" };
            var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param));
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var json = client.PostAsync(baseUrl + "KbsDeviceManage/GetElements", requestContent).Result.Content.ReadAsStringAsync().Result;
            var elements = Newtonsoft.Json.JsonConvert.DeserializeObject<ListModel<DataItemDetailEntity>>(json);
            var gridData = elements.Data.OrderBy(x => x.SortCode).Select(x => new DistrictPersonModel { CategoryId = x.ItemDetailId, CategoryName = x.ItemName }).ToList();

            var model = default(DistrictPersonModel);
            var user = OperatorProvider.Provider.Current();
            var departmentBll = new DepartmentBLL();
            var company = departmentBll.GetCompany(user.DeptId);
            var gridJson = Newtonsoft.Json.JsonConvert.SerializeObject(gridData);
            var categories = elements.Data.Select(x => new { Key = x.ItemDetailId, Value = x.ItemName });
            var categoriesJson = Newtonsoft.Json.JsonConvert.SerializeObject(categories);
            if (string.IsNullOrEmpty(id))
            {
                model = new DistrictPersonModel()
                {
                    CompanyId = company?.DepartmentId,
                    CompanyName = company?.FullName,
                };
            }
            else
            {
                var data = districtPersonBLL.GetList(id);

                if (data.Count == 0)
                {
                    model = new DistrictPersonModel()
                    {
                        CompanyId = company?.DepartmentId,
                        CompanyName = company?.FullName,
                    };
                }
                else
                {
                    var list = elements.Data.Join(data, x => x.ItemDetailId, y => y.CategoryId, (x, y) => new DistrictPersonModel
                    {
                        CategoryId = x.ItemDetailId,
                        CategoryName = x.ItemName,
                        DistrictPersonId = y == null ? null : y.DistrictPersonId,
                        DistrictId = y == null ? null : y.DistrictId,
                        DistrictCode = y == null ? null : y.DistrictCode,
                        DistrictName = y == null ? null : y.DistrictName,
                        CompanyId = y == null ? null : y.CompanyId,
                        CompanyName = y == null ? null : y.CompanyName,
                        DutyDepartmentId = y == null ? null : y.DutyDepartmentId,
                        DutyDepartmentName = y == null ? null : y.DutyDepartmentName,
                        DutyUserId = y == null ? null : y.DutyUserId,
                        DutyUser = y == null ? null : y.DutyUser,
                        Phone = y == null ? null : y.Phone,
                        Cycle = y == null ? null : y.Cycle,
                    }).ToList();
                    gridJson = Newtonsoft.Json.JsonConvert.SerializeObject(list);
                    model = new DistrictPersonModel()
                    {
                        CompanyId = data.FirstOrDefault().CompanyId,
                        CompanyName = data.FirstOrDefault().CompanyName,
                        DistrictId = data.FirstOrDefault().DistrictId,
                        DistrictCode = data.FirstOrDefault().DistrictCode,
                        DistrictName = data.FirstOrDefault().DistrictName,
                    };
                }
            }

            ViewBag.gridData = gridJson;
            ViewBag.categories = categoriesJson;
            return View(model);
        }

        [HttpPost]
        public JsonResult Edit(List<DistrictPersonModel> data)
        {
            var user = OperatorProvider.Provider.Current();
            var success = true;
            var message = "保存成功！";

            data.RemoveAll(x => string.IsNullOrEmpty(x.DutyDepartmentId));

            var entities = data.Select(x => new DistrictPersonEntity
            {
                DistrictPersonId = x.DistrictPersonId,
                DistrictId = x.DistrictId,
                DistrictCode = x.DistrictCode,
                DistrictName = x.DistrictName,
                DutyDepartmentId = x.DutyDepartmentId,
                DutyDepartmentName = x.DutyDepartmentName,
                DutyUserId = x.DutyUserId,
                DutyUser = x.DutyUser,
                CompanyId = x.CompanyId,
                CompanyName = x.CompanyName,
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryName,
                Cycle = x.Cycle.Trim(),
                Phone = x.Phone
            }).ToList();
            foreach (var item in entities)
            {
                if (string.IsNullOrEmpty(item.DistrictPersonId)) item.DistrictPersonId = Guid.NewGuid().ToString();

                item.CreateDate = DateTime.Now;
                item.UpdateDate = DateTime.Now;
                item.CreateUserId = item.UpdateUserId = user.UserId;
            }

            districtPersonBLL.Save(entities);

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }

        [HttpPost]
        public JsonResult Remove(string id)
        {
            districtPersonBLL.Remove(id);
            return Json(new AjaxResult() { type = ResultType.success, message = "删除成功！" });
        }

        public ViewResult Import()
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

            var param2 = new { Data = "区域责任人设置" };
            requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param2));
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            json = client.PostAsync($"{baseUrl}KbsDeviceManage/GetElements", requestContent).Result.Content.ReadAsStringAsync().Result;
            var elements = Newtonsoft.Json.JsonConvert.DeserializeObject<ListModel<DataItemDetailEntity>>(json);

            var departments = new DepartmentBLL().GetAll();
            var userbll = new UserBLL();


            if (this.Request.Files.Count > 0)
            {
                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];

                try
                {
                    var data = new List<DistrictPersonEntity>();
                    for (int i = 3; i <= sheet.Cells.MaxDataRow; i++)
                    {
                        if (string.IsNullOrEmpty(sheet.Cells[i, 0].StringValue) && string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue) && string.IsNullOrEmpty(sheet.Cells[i, 2].StringValue) && string.IsNullOrEmpty(sheet.Cells[i, 3].StringValue) && string.IsNullOrEmpty(sheet.Cells[i, 5].StringValue))
                            continue;

                        var item = new DistrictPersonEntity();
                        item.DistrictName = sheet.Cells[i, 0].StringValue;
                        item.CategoryName = sheet.Cells[i, 1].StringValue;
                        item.DutyDepartmentName = sheet.Cells[i, 2].StringValue;
                        item.DutyUser = sheet.Cells[i, 3].StringValue;
                        item.Phone = sheet.Cells[i, 4].StringValue;
                        item.Cycle = sheet.Cells[i, 5].StringValue;

                        //验证
                        if (string.IsNullOrEmpty(item.DistrictName)) throw new Exception($"行 {i + 1} 区域名称不能为空！");
                        if (string.IsNullOrEmpty(item.CategoryName)) throw new Exception($"行 {i + 1} 责任人类别不能为空！");
                        if (string.IsNullOrEmpty(item.DutyDepartmentName)) throw new Exception($"行 {i + 1} 部门不能为空！");
                        if (string.IsNullOrEmpty(item.DutyUser)) throw new Exception($"行 {i + 1} 责任人不能为空！");
                        if (string.IsNullOrEmpty(item.Cycle)) throw new Exception($"行 {i + 1} 周期不能为空！");

                        var district = districts.Find(x => x.DistrictName == item.DistrictName);
                        if (district == null) throw new Exception($"行 {i + 1} 区域名称不存在！");
                        else
                        {
                            item.DistrictId = district.DistrictID;
                            item.DistrictCode = district.DistrictCode;
                        }

                        var category = elements.Data.Find(x => x.ItemName == item.CategoryName);
                        if (category == null) throw new Exception($"行 {i + 1} 责任人类别不存在！");
                        else
                            item.CategoryId = category.ItemDetailId;

                        var department = departments.Find(x => x.FullName == item.DutyDepartmentName);
                        if (department == null) throw new Exception($"行 {i + 1} 部门不存在！");
                        else
                            item.DutyDepartmentId = department.DepartmentId;

                        //var dutyUser = userbll.Get(item.DutyDepartmentName, item.DutyUser);
                        //if (dutyUser == null) throw new Exception($"行 {i + 1} 责任人不存在！");
                        //else
                        //    item.DutyUserId = dutyUser.UserId;

                        item.CompanyId = company.DepartmentId;
                        item.CompanyName = company.FullName;
                        item.CreateDate = DateTime.Now;
                        item.UpdateDate = DateTime.Now;
                        item.CreateUserId = item.UpdateUserId = user.UserId;
                        item.DistrictPersonId = Guid.NewGuid().ToString();

                        data.Add(item);
                    }

                    districtPersonBLL.Save(data);
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

        public JsonResult GetList(string key, string value, int rows, int page)
        {
            var districtCode = string.Empty;
            var districtName = string.Empty;
            var category = string.Empty;
            var person = string.Empty;

            if (key == "Code") districtCode = value;
            else if (key == "Name") districtName = value;
            else if (key == "Category") category = value;
            else if (key == "Person") person = value;

            var total = 0;
            var data = districtPersonBLL.GetList(districtCode, districtName, category, person, rows, page, out total);
            return Json(new { rows = data, records = total, total = Math.Ceiling((double)total / rows), page }, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Select()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetCompany(user.DeptId);
            if (dept != null)
                ViewBag.companyid = dept.DepartmentId;
            return View();
        }

        public JsonResult GetDistrict(string companyId)
        {
            var baseUrl = Config.GetValue("ErchtmsApiUrl");
            var client = new HttpClient();
            var param = new { Data = new { companyId = companyId } };
            var requestContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(param));
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var json = client.PostAsync(baseUrl + "District/GetDistrict", requestContent).Result.Content.ReadAsStringAsync().Result;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DistrictModel>>(json);
            Building(data);
            return Json(new { rows = data, records = data.Count }, JsonRequestBehavior.AllowGet);
        }

        private void Building(List<DistrictModel> data)
        {
            var root = data.Where(x => x.ParentID == "0");
            foreach (var item in root)
            {
                item.lft = idx++;
                item.Level = 0;
                //item.ParentID = null;
                item.Expanded = false;
                Building(item, data);
                item.rgt = idx++;
            }
        }

        private void Building(DistrictModel model, List<DistrictModel> data)
        {
            var children = data.Where(x => x.ParentID == model.DistrictID);
            if (children.Count() == 0)
            {
                model.IsLeaf = true;
                model.hasChildren = false;
            }
            else
            {
                model.IsLeaf = false;
                model.hasChildren = true;
            }
            model.Expanded = false;
            foreach (var item in children)
            {
                item.lft = idx++;
                item.Level = model.Level + 1;
                Building(item, data);
                item.rgt = idx++;
            }
        }
    }
}
