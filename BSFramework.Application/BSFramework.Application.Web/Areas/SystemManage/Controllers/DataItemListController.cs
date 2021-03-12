using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Entity.SystemManage.ViewModel;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 描 述：辅助资料（主要是数据字典提供独立功能管理）
    /// </summary>
    public class DataItemListController : MvcControllerBase
    {
        private DataItemBLL dataItemBLL = new DataItemBLL();
        private DataItemDetailBLL dataItemDetailBLL = new DataItemDetailBLL();
        //private DataItemCache dataItemCache = new DataItemCache();

        #region 视图功能
        /// <summary>
        /// 辅助资料管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            string ItemCode = Request["ItemCode"];
            var data = dataItemBLL.GetEntityByCode(ItemCode);
            ViewBag.itemId = data.ItemId;
            ViewBag.isTree = data.IsTree;
            return View();
        }
        /// <summary>
        /// 辅助资料表单
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
        /// 辅助列表
        /// </summary>
        /// <param name="EnCode">代码</param>
        /// <returns>返回树形列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(string EnCode)
        {
            var data = dataItemDetailBLL.GetDataItemList().Where(x => x.EnCode == EnCode);//dataItemCache.GetDataItemList(EnCode);
            var TreeList = new List<TreeGridEntity>();
            foreach (DataItemModel item in data)
            {
                TreeGridEntity tree = new TreeGridEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.ItemDetailId) == 0 ? false : true;
                tree.id = item.ItemDetailId;
                tree.parentId = item.ParentId;
                tree.expanded = true;
                tree.hasChildren = hasChildren;
                tree.entityJson = item.ToJson();
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeJson());
        }
        /// <summary>
        /// 辅助实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = dataItemDetailBLL.GetEntity(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除辅助
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除辅助资料")]
        public ActionResult RemoveForm(string keyValue)
        {
            dataItemDetailBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存辅助表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataItemDetailEntity">辅助实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存辅助资料(新增、修改)")]
        public ActionResult SaveForm(string keyValue, DataItemDetailEntity dataItemDetailEntity)
        {
            dataItemDetailBLL.SaveForm(keyValue, dataItemDetailEntity);
            return Success("操作成功。");
        }
        #endregion
    }
}
