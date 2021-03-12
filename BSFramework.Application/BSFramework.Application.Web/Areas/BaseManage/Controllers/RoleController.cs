using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.BaseManage.Controllers
{
    /// <summary>
    /// 描 述：角色管理
    /// </summary>
    public class RoleController : MvcControllerBase
    {
        private RoleBLL roleBLL = new RoleBLL();
        //private RoleCache roleCache = new RoleCache();

        #region 视图功能
        /// <summary>
        /// 角色管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 角色表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 选择角色
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Ignore)]
        public ActionResult Select()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="organizeId">公司Id</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(string organizeId)
        {
            var data = roleBLL.GetList(organizeId);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="rolename">角色名</param>
        /// <param name="rows">查询参数</param>
        /// <param name="page">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public JsonResult GetPageListJson(string rolename, int rows, int page)
        {
            var total = 0;
            var data = roleBLL.GetPageList(rolename, rows, page, out total);

            return Json(new { rows = data, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);


            //var watch = CommonHelper.TimerStart();
            //var data = roleBLL.GetPageList(pagination, queryJson);
            //var JsonData = new
            //{
            //    rows = data,
            //    total = pagination.total,
            //    page = pagination.page,
            //    records = pagination.records,
            //    costtime = CommonHelper.TimerEnd(watch)
            //};
            //return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 角色实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = roleBLL.GetEntity(keyValue);
            return Content(data.ToJson("yyyy-MM-dd HH:mm"));
        }

        #region 根据查询条件获取角色树

        /// <summary>
        /// 根据查询条件获取部门树 
        /// </summary>
        /// <param name="Ids">上级部门(机构)Id</param>
        /// <param name="checkMode">单选或多选，0:单选，1:多选</param>
        /// <param name="mode">查询模式，0:获取机构ID为Ids下所有的部门(即OrganizeId=Ids)，1:获取部门ID为Ids下所有部门(即ParentId=Ids)，2:获取部门的parentId在Ids中的部门(即ParentId in(Ids))，3:获取部门Id在Ids中的部门(即DepartmentId in(Ids))</param>
        /// <param name="roleIDs">角色IDs</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetRoleTreeJson(string Ids, string roleIDs, int checkMode = 0, int mode = 0)
        {
            Operator user = OperatorProvider.Provider.Current();
            //不传Ids那么显示所有通用角色，选择Ids显示当前公司下面的加上所有通用角色
            string parentId = "0";
            var treeList = new List<TreeEntity>();
            IEnumerable<RoleEntity> data = new List<RoleEntity>();
            //如果没有传递参数parentId,则给出默认值
            if (string.IsNullOrEmpty(Ids))
            {
                data = new RoleBLL().GetList().Where(x => x.IsPublic == 1);//roleCache.GetList().Where(a => a.IsPublic==1);
            }
            else
            {
                parentId = Ids;
                if (user.IsSystem)
                {
                    data = new RoleBLL().GetList();
                }
                else
                {
                    data = new RoleBLL().GetList().Where(x => x.IsPublic == 1 || x.OrganizeId == parentId);
                }
                //roleCache.GetList().Where(a => a.IsPublic==1|| a.OrganizeId == parentId );

            }
            foreach (RoleEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = false;
                tree.id = item.RoleId;
                tree.text = item.FullName;
                tree.value = item.RoleId;
                tree.isexpand = true;
                tree.complete = true;
                if (!string.IsNullOrEmpty(roleIDs))
                {
                    if (roleIDs.Contains(item.RoleId)) tree.checkstate = 1;
                }
                tree.showcheck = checkMode == 0 ? false : true;
                tree.hasChildren = hasChildren;
                tree.parentId = parentId;
                tree.Attribute = "Code";
                tree.AttributeValue = item.EnCode;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson(parentId));
        }
        #endregion
        #endregion

        #region 验证数据
        /// <summary>
        /// 角色编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistEnCode(string EnCode, string keyValue)
        {
            bool IsOk = roleBLL.ExistEnCode(EnCode, keyValue);
            return Content(IsOk.ToString());
        }
        /// <summary>
        /// 角色名称不能重复
        /// </summary>
        /// <param name="FullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistFullName(string FullName, string keyValue)
        {
            bool IsOk = roleBLL.ExistFullName(FullName, keyValue);
            return Content(IsOk.ToString());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除角色信息")]
        public ActionResult RemoveForm(string keyValue)
        {
            Erchtms e = new Erchtms();
            RoleEntity roleEntity = roleBLL.GetEntity(keyValue);
            e.ErchtmsSynchronoous("DeleteRole", roleEntity, "");
            roleBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存角色表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="roleEntity">角色实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)角色信息")]
        public ActionResult SaveForm(string keyValue, RoleEntity roleEntity)
        {
            roleBLL.SaveForm(keyValue, roleEntity);
            Erchtms e = new Erchtms();
            e.ErchtmsSynchronoous("SaveRole", roleEntity, "");
            return Success("操作成功。");
        }
        #endregion
    }
}
