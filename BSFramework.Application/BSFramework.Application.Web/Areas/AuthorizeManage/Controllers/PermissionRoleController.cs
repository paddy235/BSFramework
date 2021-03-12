using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Busines.AuthorizeManage;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.AuthorizeManage.Controllers
{
    /// <summary>
    /// 描 述：角色权限
    /// </summary>
    public class PermissionRoleController : MvcControllerBase
    {
        private OrganizeBLL organizeBLL = new OrganizeBLL();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        //private DepartmentCache departmentCache = new DepartmentCache();
        private RoleBLL roleBLL = new RoleBLL();
        private UserBLL userBLL = new UserBLL();
        private PermissionBLL permissionBLL = new PermissionBLL();
        private AuthorizeBLL authorizeBLL = new AuthorizeBLL();

        #region 视图功能
        /// <summary>
        /// 角色权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult AllotAuthorize()
        {
            return View();
        }
        /// <summary>
        /// 角色成员
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult AllotMember()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 部门列表 
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetDepartmentTreeJson(string roleId)
        {
            var roleEntity = roleBLL.GetEntity(roleId);
            var organizeEntity = organizeBLL.GetEntity(roleEntity.OrganizeId);
            var data = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey); //departmentCache.GetList(roleEntity.OrganizeId);
            if (data == null)
            {
                data = departmentBLL.GetList();
            }
            data = data.Where(x => x.OrganizeId == roleEntity.OrganizeId);

            var treeList = new List<TreeEntity>();
            TreeEntity tree = new TreeEntity();
            if (organizeEntity!=null)
            {
                 tree.id = organizeEntity.OrganizeId;
                 tree.text = organizeEntity.FullName;
                 tree.value = organizeEntity.OrganizeId;
            }
           
            tree.isexpand = true;
            tree.complete = true;
            tree.hasChildren = true;
            tree.parentId = "0";
            treeList.Add(tree);
            foreach (DepartmentEntity item in data)
            {
                tree = new TreeEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.DepartmentId) == 0 ? false : true;
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                if (item.ParentId == "0")
                {
                    tree.parentId = roleEntity.OrganizeId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUserListJson(string roleId)
        {
            var existMember = permissionBLL.GetMemberList(roleId);
            var userdata = userBLL.GetTable();
            if (userdata != null && userdata.Count > 0)
            {
                var datalist = userdata.Select(x =>
                {
                    int ischeck = existMember.Count(t => t.UserId == x.UserId) > 0 ? 1 : 0;
                    int? IsDefault = 0;
                    if (ischeck > 0)
                    {
                        IsDefault = existMember.First(t => t.UserId == x.UserId).IsDefault;
                    }

                    var data = new
                    {
                        x.Account,
                        x.Age,
                        x.AllowEndTime,
                        x.AllowStartTime,
                        x.AnswerQuestion,
                        x.Birthday,
                        x.CheckOnLine,
                        x.Craft,
                        x.CraftAge,
                        x.CreateDate,
                        x.CreateUserId,
                        x.CreateUserName,
                        x.CurrentWorkAge,
                        x.Degrees,
                        x.DeleteMark,
                        x.DepartmentCode,
                        x.DepartmentId,
                        x.DepartmentName,
                        x.Description,
                        x.DutyId,
                        x.DutyName,
                        x.Email,
                        x.EnabledMark,
                        x.EnCode,
                        x.EnterTime,
                        x.FINGER,
                        x.FirstVisit,
                        x.Folk,
                        x.FourPersonType,
                        x.Gender,
                        x.HeadIcon,
                        x.HealthStatus,
                        x.IDENTIFYID,
                        x.IsEpiboly,
                        x.IsFourPerson,
                        x.IsSpecial,
                        x.IsSpecialEqu,
                        x.JobName,
                        x.JobTitle,
                        x.LabourNo,
                        x.LastVisit,
                        x.LateDegrees,
                        x.LockEndDate,
                        x.LockStartDate,
                        x.LogOnCount,
                        x.Manager,
                        x.ManagerId,
                        x.Mobile,
                        x.ModifyDate,
                        x.ModifyUserId,
                        x.ModifyUserName,
                        x.MSN,
                        x.Nation,
                        x.Native,
                        x.NewDegree,
                        x.NickName,
                        x.OICQ,
                        x.OldDegree,
                        x.OpenId,
                        x.OrganizeCode,
                        x.OrganizeId,
                        x.Password,
                        x.Photo,
                        x.Planer,
                        x.Political,
                        x.PostCode,
                        x.PostId,
                        x.PostName,
                        x.PreviousVisit,
                        x.Quarters,
                        x.Question,
                        x.QuickQuery,
                        x.RealName,
                        x.RoleId,
                        x.RoleName,
                        x.Secretkey,
                        x.SecurityLevel,
                        x.Signature,
                        x.SignImg,
                        x.SimpleSpelling,
                        x.SortCode,
                        x.SpecialtyType,
                        x.TechnicalGrade,
                        x.TecLevel,
                        x.Telephone,
                        x.UserId,
                        x.UserOnLine,
                        x.UserType,
                        x.Visage,
                        x.WeChat,
                        x.WorkGroupId,
                        x.WorkKind,
                        isdefault = IsDefault,
                        ischeck = ischeck
                    };
                    return data;
                }).OrderByDescending(x => x.ischeck).ToList();
                return Content(datalist.ToJson());
            }
            return Content(userdata.ToJson());
        }
        /// <summary>
        /// 系统功能列表
        /// </summary>
        /// <param name="RoleId">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ModuleTreeJson(string roleId)
        {
            var existModule = permissionBLL.GetModuleList(roleId);
            var data = authorizeBLL.GetModuleList(SystemInfo.CurrentUserId);
            var treeList = new List<TreeEntity>();
            foreach (ModuleEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.ModuleId) == 0 ? false : true;
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                tree.title = "";
                tree.checkstate = existModule.Count(t => t.ItemId == item.ModuleId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.img = item.Icon;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 系统按钮列表
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ModuleButtonTreeJson(string roleId)
        {
            var existModuleButton = permissionBLL.GetModuleButtonList(roleId);
            var moduleData = authorizeBLL.GetModuleList(SystemInfo.CurrentUserId);
            var moduleButtonData = authorizeBLL.GetModuleButtonList(SystemInfo.CurrentUserId);
            var treeList = new List<TreeEntity>();
            foreach (ModuleEntity item in moduleData)
            {
                
                //if (item.IsMenu == 1)
                //{
                //    bool hasChildren = moduleButtonData.Count(t => t.ModuleId == item.ModuleId) == 0 ? false : true;
                //    if (hasChildren == false)
                //    {
                //        continue;
                //    }
                //}
                TreeEntity tree = new TreeEntity();
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                tree.checkstate = existModuleButton.Count(t => t.ItemId == item.ModuleId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                tree.parentId = item.ParentId;
                tree.img = item.Icon;
                treeList.Add(tree);
            }
            foreach (ModuleButtonEntity item in moduleButtonData)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = moduleButtonData.Count(t => t.ParentId == item.ModuleButtonId) == 0 ? false : true;
                tree.id = item.ModuleButtonId;
                tree.text = item.FullName;
                tree.value = item.ModuleButtonId;
                if (item.ParentId == "0")
                {
                    tree.parentId = item.ModuleId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.checkstate = existModuleButton.Count(t => t.ItemId == item.ModuleButtonId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.img = "fa fa-wrench " + item.ModuleId;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 系统视图列表
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ModuleColumnTreeJson(string roleId)
        {
            var existModuleColumn = permissionBLL.GetModuleColumnList(roleId);
            var moduleData = authorizeBLL.GetModuleList(SystemInfo.CurrentUserId);
            var moduleColumnData = authorizeBLL.GetModuleColumnList(SystemInfo.CurrentUserId);
            var treeList = new List<TreeEntity>();
            foreach (ModuleEntity item in moduleData)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                tree.checkstate = existModuleColumn.Count(t => t.ItemId == item.ModuleId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                tree.parentId = item.ParentId;
                tree.img = item.Icon;
                treeList.Add(tree);
            }
            foreach (ModuleColumnEntity item in moduleColumnData)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = moduleColumnData.Count(t => t.ParentId == item.ModuleColumnId) == 0 ? false : true;
                tree.id = item.ModuleColumnId;
                tree.text = item.FullName;
                tree.value = item.ModuleColumnId;
                if (item.ParentId == "0")
                {
                    tree.parentId = item.ModuleId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.checkstate = existModuleColumn.Count(t => t.ItemId == item.ModuleColumnId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.img = "fa fa-filter " + item.ModuleId;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 数据权限列表
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrganizeTreeJson(string roleId)
        {
            var existAuthorizeData = permissionBLL.GetAuthorizeDataList(roleId);
            var organizedata = organizeBLL.GetList();
            var departmentdata = departmentBLL.GetList();
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                    if (hasChildren == false)
                    {
                        continue;
                    }
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                if (item.ParentId == "0")
                {
                    tree.img = "fa fa-sitemap";
                }
                else
                {
                    tree.img = "fa fa-home";
                }
                tree.checkstate = existAuthorizeData.Count(t => t.ResourceId == item.OrganizeId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = departmentdata.Count(t => t.ParentId == item.DepartmentId) == 0 ? false : true;
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                if (item.ParentId == "0")
                {
                    tree.parentId = item.OrganizeId;
                }
                else
                {
                    tree.parentId = item.ParentId;
                }
                tree.checkstate = existAuthorizeData.Count(t => t.ResourceId == item.DepartmentId);
                tree.showcheck = true;
                tree.isexpand = true;
                tree.complete = true;
                tree.img = "fa fa-umbrella";
                tree.hasChildren = hasChildren;
                treeList.Add(tree);
            }
            int authorizeType = -1;
            if (existAuthorizeData.ToList().Count > 0)
            {
                authorizeType = existAuthorizeData.ToList()[0].AuthorizeType.ToInt();
            }
            var JsonData = new
            {
                authorizeType = authorizeType,
                authorizeData = existAuthorizeData,
                treeJson = treeList.TreeToJson(),
            };
            return Content(JsonData.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存角色成员
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="userIds">成员Id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)角色成员信息")]
        public ActionResult SaveMember(string roleId, string userIds)
        {
            permissionBLL.SaveMember((int)AuthorizeTypeEnum.Role, roleId, userIds);
            return Success("保存成功。");
        }
        /// <summary>
        /// 保存角色授权
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleIds">功能Id</param>
        /// <param name="moduleButtonIds">按钮Id</param>
        /// <param name="moduleColumnIds">视图Id</param>
        /// <param name="authorizeDataJson">数据权限</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)角色授权信息")]
        public ActionResult SaveAuthorize(string roleId, string moduleIds, string moduleButtonIds, string moduleColumnIds, string authorizeDataJson)
        {
            permissionBLL.SaveAuthorize((int)AuthorizeTypeEnum.Role, roleId, moduleIds, moduleButtonIds, moduleColumnIds, authorizeDataJson);
            return Success("保存成功。");
        }
        #endregion
    }
}
