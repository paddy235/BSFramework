using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.Entity.ProducingCheck;
using BSFramework.Application.Web.Areas.ProducingCheck.Models;
using BSFramework.Busines.EvaluateAbout;
using BSFramework.Busines.ProducingCheck;
using BSFramework.Entity.EvaluateAbout;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.ProducingCheck.Controllers
{
    /// <summary>
    /// 安全文明生产检查问题类型
    /// </summary>
    public class CheckCategoryController : MvcControllerBase
    {
        private CheckCategoryBLL checkCategoryBLL = new CheckCategoryBLL();


        /// <summary>
        /// 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            return View();
        }

        /// <summary>
        /// 获取类别
        /// </summary>
        /// <returns></returns>
        public JsonResult Tree()
        {
            var data = checkCategoryBLL.GetCategories().ToList();

            return Json(data.Where(x => x.ParentId == null).Select(x => new TreeModel { id = x.CategoryId, value = x.CategoryId, text = x.CategoryName, isexpand = data.Count(y => y.ParentId == x.CategoryId) > 0, hasChildren = data.Count(y => y.ParentId == x.CategoryId) > 0, ChildNodes = GetChildren(data, x.CategoryId) }).ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<CheckCategoryEntity> data, string id)
        {
            return data.Where(x => x.ParentId == id).OrderBy(x => x.CreateDate).Select(x => new TreeModel { id = x.CategoryId, value = x.CategoryId, text = x.CategoryName, isexpand = data.Count(y => y.ParentId == x.CategoryId) > 0, hasChildren = data.Count(y => y.ParentId == x.CategoryId) > 0, ChildNodes = GetChildren(data, x.CategoryId) }).ToList();
        }

        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public ViewResult Edit(string id, string parentid)
        {
            var entity = checkCategoryBLL.Get(id);
            var parent = checkCategoryBLL.Get(parentid);
            var model = new CategoryModel();
            if (entity == null)
            {
                if (parent != null)
                    model.ParentCategory = new CategoryModel { Category = parent.CategoryName, CategoryId = parent.CategoryId };
            }
            else
            {
                model.CategoryId = entity.CategoryId;
                model.Category = entity.CategoryName;
                if (entity.ParentCategory != null)
                    model.ParentCategory = new CategoryModel { Category = entity.ParentCategory.CategoryName, CategoryId = entity.ParentCategory.CategoryId };
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
        public JsonResult Edit(string id, CategoryModel model)
        {
            var success = true;
            var message = "保存成功";

            try
            {
                checkCategoryBLL.Edit(new CheckCategoryEntity()
                {
                    CategoryId = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id,
                    CategoryName = model.Category,
                    CreateDate = DateTime.Now,
                    ParentId = model.ParentCategory.CategoryId
                }); ;
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
            var success = true;
            var message = "删除成功";

            try
            {
                checkCategoryBLL.Delete(id);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }
    }
}
