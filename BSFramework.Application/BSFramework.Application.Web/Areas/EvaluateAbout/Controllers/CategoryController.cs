using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Web.Areas.EvaluateAbout.Models;
using BSFramework.Busines.EvaluateAbout;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ThoughtWorks.QRCode.Codec;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class CategoryController : MvcControllerBase
    {
        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            bool canAdd = false;
            if (user.IsSystem || ConfigurationManager.AppSettings["EvaluateDept"].ToString().Contains(user.DeptName))
            {
                canAdd = true;
            }
            ViewBag.allowadd = canAdd;
            return View();
        }

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult GetCategories()
        {
            var bll = new EvaluateBLL();
            var data = bll.GetAllCategories().ToList();

            return Json(data.Where(x => x.ParentCategoryId == null).Select(x => new TreeModel { id = x.CategoryId, value = x.CategoryId, text = x.Category, isexpand = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, hasChildren = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, ChildNodes = GetChildren(data, x.CategoryId) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<EvaluateCategoryEntity> data, string id)
        {
            return data.Where(x => x.ParentCategoryId == id).OrderBy(x => x.SortCode).Select(x => new TreeModel { id = x.CategoryId, value = x.CategoryId, text = x.Category, isexpand = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, hasChildren = data.Count(y => y.ParentCategoryId == x.CategoryId) > 0, ChildNodes = GetChildren(data, x.CategoryId) }).ToList();
        }

        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(string id)
        {
            return View();
        }

      
        /// <summary>
        /// 考评标准类别修改
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ClassEdit(string id)
        {
            var bll = new EvaluateBLL();

            var list = bll.GetAllCategories().ToList();
            var data = list.Where(t => t.CategoryId == id).LastOrDefault();
            var parentCategory = list.Where(t => t.CategoryId == data.ParentCategoryId).LastOrDefault();

            CategoryModel entity = new CategoryModel();
            if (parentCategory != null)
            {
                ViewData["parentCategory"] = parentCategory.Category;
                ViewData["parentCategoryId"] = parentCategory.CategoryId;
            }
            else
            {
                ViewData["parentCategory"] = "";
                ViewData["parentCategoryId"] = "";
            }
            return View(new CategoryModel() { Category = data.Category, CategoryId = data.CategoryId, SortCode = data.SortCode });
        }
        /// <summary>
        /// 考评标准类别修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ClassEdit(string id, CategoryModel model)
        {
            var bll = new EvaluateBLL();
            var weight = new WeightSetBLL();
            var success = true;
            var message = "保存成功";
            try
            {
                bll.Edit(new EvaluateCategoryEntity() { Category = model.Category, CategoryId = model.CategoryId, ParentCategoryId = model.ParentCategory == null ? null : model.ParentCategory.ToString(),SortCode=model.SortCode, CreateTime = DateTime.Now });

                //同步修改班组权重表信息
                WeightSetEntity entity = weight.GetEntity(id);
                if (entity != null)
                {
                    entity.ClassName = model.Category;
                    weight.SaveForm(id, entity);
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }
            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit(string id, CategoryModel model)
        {
            var bll = new EvaluateBLL();

            var success = true;
            var message = "保存成功";

            try
            {
                if (string.IsNullOrEmpty(id))
                    bll.Add(new EvaluateCategoryEntity() { Category = model.Category, CreateTime = DateTime.Now, ParentCategoryId = model.ParentCategory == null ? null : model.ParentCategory.CategoryId });
                else
                    bll.Edit(new EvaluateCategoryEntity() { Category = model.Category, CreateTime = DateTime.Now, ParentCategoryId = model.ParentCategory.CategoryId });
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
              

        /// <summary>
        /// 选择页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Select()
        {
            return View();
        }

        /// <summary>
        /// 删除类别
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string id)
        {
            var bll = new EvaluateBLL();
            var weight = new WeightSetBLL();
            var success = true;
            var message = "删除成功";

            try
            {
                bll.DeleteCategory(id);
                //同步删除班组权重表信息
                WeightSetEntity entity = weight.GetEntity(id);
                if (entity != null)
                {
                    weight.RemoveForm(id);
                }
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
        public JsonResult DeleteItem(string id)
        {
            var bll = new EvaluateBLL();

            var success = true;
            var message = "删除成功";

            try
            {
                bll.DeleteItem(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        /// <summary>
        /// 获取考评内容
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetData(int rows, int page, string key, string categoryid)
        {
            var bll = new EvaluateBLL();
            var total = 0;
            var data = bll.GetCategoryItems(key, categoryid, rows, page, out total).Select(x => new { ItemId = x.ItemId, Category = x.Category.Category, ItemContent = x.ItemContent, ItemStandard = x.ItemStandard, EvaluateDept = x.EvaluateDept, UseDept = x.UseDept, Score = x.Score });
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 编辑考评内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit2(string id)
        {
            CategoryItemModel model = null;
            if (!string.IsNullOrEmpty(id))
            {
                var bll = new EvaluateBLL();
                var data = bll.GetCategoryItem(id);
                model = new CategoryItemModel() { Category = new CategoryModel() { CategoryId = data.Category.CategoryId, Category = data.Category.Category }, ItemId = data.ItemId, ItemContent = data.ItemContent, ItemStandard = data.ItemStandard, Score = data.Score, EvaluateDept = data.EvaluateDept, UseDept = data.UseDept };
            }
            else
                model = new CategoryItemModel();

            return View(model);
        }

        /// <summary>
        /// 新增和修改考评内容
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Edit2(string id, CategoryItemModel model)
        {
            var bll = new EvaluateBLL();

            var success = true;
            var message = "保存成功";
            try
            {
                if (string.IsNullOrEmpty(id))
                    bll.AddItem(new EvaluateCategoryItemEntity() { CategoryId = model.Category.CategoryId, ItemContent = model.ItemContent, ItemStandard = model.ItemStandard, Score = model.Score, EvaluateDept = model.EvaluateDept, UseDept = model.UseDept,UseDeptId=model.UseDeptId });
                else
                    bll.EditItem(new EvaluateCategoryItemEntity() { ItemId = model.ItemId, CategoryId = model.Category.CategoryId, ItemContent = model.ItemContent, ItemStandard = model.ItemStandard, Score = model.Score, EvaluateDept = model.EvaluateDept, UseDept = model.UseDept, UseDeptId = model.UseDeptId });
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
        public JsonResult Import(string id)
        {
            DepartmentBLL dpbll = new DepartmentBLL();
            var book = new Workbook(this.Request.Files[0].InputStream);
            var sheet = book.Worksheets[0];

            var sss = sheet.Cells[0, 0].StringValue;
            var bll = new EvaluateBLL();
            var categories = bll.GetAllCategories().ToList();

            var success = true;
            var message = "保存成功";
            var user = OperatorProvider.Provider.Current();

            var date = DateTime.Now;
            try
            {
                var categoryname = string.Empty;
                var categoryitems = new List<EvaluateCategoryItemEntity>();
                for (int i = 1; i <= sheet.Cells.MaxRow; i++)
                {
                    var currentcategory = sheet.Cells[i, 0].StringValue;
                    if (!string.IsNullOrWhiteSpace(currentcategory))
                        categoryname = currentcategory;

                    var category = categories.Find(x => x.Category == categoryname);
                    if (category == null) throw new Exception(string.Format("不存在考评要素，行 {0}", i + 1));

                    var score = 0;
                    if (sheet.Cells[i, 3].Type == CellValueType.IsNumeric)
                        score = sheet.Cells[i, 3].IntValue;

                    var itemcontent = sheet.Cells[i, 1].StringValue;
                    if (string.IsNullOrEmpty(itemcontent)) throw new Exception(string.Format("考评内容不能为空，行 {0}", i + 1));

                    var khbz = sheet.Cells[i, 2].StringValue;
                    if (string.IsNullOrEmpty(khbz)) throw new Exception(string.Format("考评标准不能为空，行 {0}", i + 1));

                    var deptname = sheet.Cells[i, 5].StringValue;
                    var deptid = "";
                    if (!string.IsNullOrEmpty(deptname))
                    {

                        if (deptname.Contains(','))
                        {
                            string[] names = deptname.Split(',');
                            foreach (string name in names)
                            {
                                var dept = dpbll.GetList().Where(x => x.FullName == name && x.Nature == "班组").FirstOrDefault();
                                if (dept == null)
                                {
                                    throw new Exception(string.Format("班组{1}不存在，行 {0}", i + 1, deptname));
                                }
                                else
                                {
                                    deptid += dept.DepartmentId + ",";
                                }
                            }
                            if (deptid.EndsWith(",")) deptid = deptid.Substring(0, deptid.Length - 1);
                        }
                        else
                        {
                            var dept = dpbll.GetList().Where(x => x.FullName == deptname && x.Nature == "班组").FirstOrDefault();
                            if (dept == null)
                            {
                                throw new Exception(string.Format("班组{1}不存在，行 {0}", i + 1, deptname));
                            }
                            else
                            {
                                deptid = dept.DepartmentId;
                            }
                        }
                    }
                    
                    
                    var categoryitem = new EvaluateCategoryItemEntity() { ItemId = Guid.NewGuid().ToString(), CategoryId = category.CategoryId, ItemContent = sheet.Cells[i, 1].StringValue, ItemStandard = sheet.Cells[i, 2].StringValue, EvaluateDept = sheet.Cells[i, 4].StringValue, UseDept = deptname,UseDeptId=deptid, CreateUserId = user.UserId, CreateTime = date.AddSeconds(i), Score = score };
                    categoryitems.Add(categoryitem);
                }

                bll.AddItem(categoryitems);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }

        public ActionResult Download()
        {
            var book = new Workbook();
            var sheet = book.Worksheets[0];
            sheet.Name = "考评内容";
            sheet.Cells[0, 0].PutValue("考评要素");
            sheet.Cells[0, 1].PutValue("考评内容");
            sheet.Cells[0, 2].PutValue("考评标准");
            sheet.Cells[0, 3].PutValue("标准分");
            sheet.Cells[0, 4].PutValue("考评部门");
            sheet.Cells[0, 5].PutValue("适用部门");

            sheet.Cells.SetRowHeightPixel(0, 22);

            sheet.Cells.SetColumnWidthPixel(0, 100);
            sheet.Cells.SetColumnWidthPixel(1, 300);
            sheet.Cells.SetColumnWidthPixel(2, 300);
            sheet.Cells.SetColumnWidthPixel(3, 60);
            sheet.Cells.SetColumnWidthPixel(4, 100);
            sheet.Cells.SetColumnWidthPixel(5, 100);

            book.Save(System.Web.HttpContext.Current.Response, "导入模板.xlsx", ContentDisposition.Attachment, new XlsSaveOptions(SaveFormat.Xlsx));
            return View();
        }

    }
}
