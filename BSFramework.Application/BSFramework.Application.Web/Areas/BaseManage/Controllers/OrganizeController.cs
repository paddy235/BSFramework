using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.BaseManage.Controllers
{
    /// <summary>
    /// 描 述：机构管理
    /// </summary>
    public class OrganizeController : MvcControllerBase
    {
        private OrganizeBLL organizeBLL = new OrganizeBLL();
        //private OrganizeCache organizeCache = new OrganizeCache();

        #region 视图功能
        /// <summary>
        /// 机构管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 机构表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 机构列表 
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetTreeJson(string keyword)
        {
            var data = organizeBLL.GetList().ToList();//organizeCache.GetList().ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.FullName.Contains(keyword), "OrganizeId");
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 机构列表 
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形列表Json</returns>
        [HttpGet]
        public ActionResult GetTreeListJson(string condition, string keyword)
        {
            var data = organizeBLL.GetList().ToList();
            if (!string.IsNullOrEmpty(condition) && !string.IsNullOrEmpty(keyword))
            {
                #region 多条件查询
                switch (condition)
                {
                    case "FullName":    //公司名称
                        data = data.TreeWhere(t => t.FullName.Contains(keyword), "OrganizeId");
                        break;
                    case "EnCode":      //外文名称
                        data = data.TreeWhere(t => t.EnCode.Contains(keyword), "OrganizeId");
                        break;
                    case "ShortName":   //中文名称
                        data = data.TreeWhere(t => t.ShortName.Contains(keyword), "OrganizeId");
                        break;
                    case "Manager":     //负责人
                        data = data.TreeWhere(t => t.Manager.Contains(keyword), "OrganizeId");
                        break;
                    default:
                        break;
                }
                #endregion
            }
            var treeList = new List<TreeGridEntity>();
            foreach (OrganizeEntity item in data)
            {
                TreeGridEntity tree = new TreeGridEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                tree.id = item.OrganizeId;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.expanded = true;
                tree.entityJson = item.ToJson();
                treeList.Add(tree);
            }
            return Content(treeList.TreeJson());
        }
        /// <summary>
        /// 机构实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = organizeBLL.GetEntity(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 公司名称不能重复
        /// </summary>
        /// <param name="organizeName">公司名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistFullName(string FullName, string keyValue)
        {
            bool IsOk = organizeBLL.ExistFullName(FullName, keyValue);
            return Content(IsOk.ToString());
        }
        /// <summary>
        /// 外文名称不能重复
        /// </summary>
        /// <param name="enCode">外文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistEnCode(string EnCode, string keyValue)
        {
            bool IsOk = organizeBLL.ExistEnCode(EnCode, keyValue);
            return Content(IsOk.ToString());
        }
        /// <summary>
        /// 中文名称不能重复
        /// </summary>
        /// <param name="shortName">中文名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistShortName(string ShortName, string keyValue)
        {
            bool IsOk = organizeBLL.ExistShortName(ShortName, keyValue);
            return Content(IsOk.ToString());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除机构
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "删除机构信息")]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RemoveForm(string keyValue)
        {
            var deptlist = new DepartmentBLL().GetList();
            bool hasChildren = deptlist.Count(t => t.OrganizeId == keyValue) == 0 ? false : true;
            if (hasChildren == true)
            {
                return Success("删除失败。");
            }
            else
            {
                organizeBLL.RemoveForm(keyValue);
                return Success("删除成功。");
            }
        }
        /// <summary>
        /// 保存机构表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="organizeEntity">机构实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)机构信息")]
        public ActionResult SaveForm(string keyValue, OrganizeEntity organizeEntity)
        {

            organizeBLL.SaveForm(keyValue, organizeEntity);
            return Success("操作成功。");
        }
        #endregion
    }
}
