using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Busines.AuthorizeManage;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.AuthorizeManage.Controllers
{
    /// <summary>
    /// 描 述：职位权限
    /// </summary>
    public class PermissionJobController : MvcControllerBase
    {
        private OrganizeBLL organizeBLL = new OrganizeBLL();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        private RoleBLL roleBLL = new RoleBLL();
        private UserBLL userBLL = new UserBLL();
        private ModuleBLL moduleBLL = new ModuleBLL();
        private ModuleButtonBLL moduleButtonBLL = new ModuleButtonBLL();
        private ModuleColumnBLL moduleColumnBLL = new ModuleColumnBLL();
        private PermissionBLL permissionBLL = new PermissionBLL();

        #region 视图功能
        /// <summary>
        /// 职位权限
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult AllotAuthorize()
        {
            return View();
        }
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult AllotAuthorizeNew()
        {
            return View();
        }
        /// <summary>
        /// 职位成员
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
        /// 用户列表
        /// </summary>
        /// <param name="departmentId">部门Id</param>
        /// <param name="jobId">职位Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetUserListJson(string departmentId, string jobId)
        {
            var existMember = permissionBLL.GetMemberList(jobId);
            var userdata = userBLL.GetTable().Where(x => x.DepartmentId == departmentId).ToList();
            if (userdata !=null && userdata.Count>0)
            {
             var datalist =    userdata.Select(x =>
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
                }).OrderByDescending(x =>x.ischeck).ToList();
                return Content(datalist.ToJson());
            }
            return Content(userdata.ToJson());
        }
        /// <summary>
        /// 系统功能列表
        /// </summary>
        /// <param name="jobId">职位Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ModuleTreeJson(string jobId)
        {
            var existModule = permissionBLL.GetModuleList(jobId);
            var data = moduleBLL.GetList();
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
        /// <param name="jobId">职位Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ModuleButtonTreeJson(string jobId)
        {
            var existModuleButton = permissionBLL.GetModuleButtonList(jobId);
            var moduleData = moduleBLL.GetList();
            var moduleButtonData = moduleButtonBLL.GetList();
            var treeList = new List<TreeEntity>();
            foreach (ModuleEntity item in moduleData)
            {
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
        /// <param name="jobId">职位Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ModuleColumnTreeJson(string jobId)
        {
            var existModuleColumn = permissionBLL.GetModuleColumnList(jobId);
            var moduleData = moduleBLL.GetList();
            var moduleColumnData = moduleColumnBLL.GetList();
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
        /// <param name="jobId">职位Id</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OrganizeTreeJson(string jobId)
        {
            var existAuthorizeData = permissionBLL.GetAuthorizeDataList(jobId);
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
        /// 保存职位成员
        /// </summary>
        /// <param name="jobId">职位Id</param>
        /// <param name="userIds">成员Id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)职位成员信息")]
        public ActionResult SaveMember(string jobId, string userIds)
        {
            permissionBLL.SaveMember((int)AuthorizeTypeEnum.Job, jobId, userIds);
            return Success("保存成功。");
        }
        /// <summary>
        /// 保存职位授权
        /// </summary>
        /// <param name="jobId">职位Id</param>
        /// <param name="moduleIds">功能Id</param>
        /// <param name="moduleButtonIds">按钮Id</param>
        /// <param name="moduleColumnIds">视图Id</param>
        /// <param name="authorizeDataJson">数据权限</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)职位授权信息")]
        public ActionResult SaveAuthorize(int authorizeType, string jobId, string moduleIds, string moduleButtonIds, string moduleColumnIds, string authorizeDataJson)
        {
            int type = (int)AuthorizeTypeEnum.Role;
            switch (authorizeType)
            {
                case 1:
                    type = (int)AuthorizeTypeEnum.Department;
                    break;
                case 3:
                    type = (int)AuthorizeTypeEnum.Post;
                    break;
                case 4:
                    type = (int)AuthorizeTypeEnum.Job;
                    break;
                case 5:
                    type = (int)AuthorizeTypeEnum.User;
                    break;
                case 6:
                    type = (int)AuthorizeTypeEnum.UserGroup;
                    break;
            }
            permissionBLL.SaveAuthorize(type, jobId, moduleIds, moduleButtonIds.TrimEnd(','), moduleColumnIds.TrimEnd(','), authorizeDataJson);
            return Success("保存成功。");
        }
        /// <summary>
        /// 获取用户对模块的数据的修改和删除权限（本人,本部门，本部门及下属部门，本机构，全部）
        /// </summary>
        ///<param name="jsonData">json集合字符串，如[{UserId:'1',DeptCode:'0001',OrgCode:'00'},{UserId:'2',DeptCode:'0002',OrgCode:'00'}]</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "获取当前用户对当前模块的数据权限")]
        public string GetDataAuthority(string jsonData)
        {
            var user = OperatorProvider.Provider.Current();
            AuthorizeBLL authBLL = new AuthorizeBLL();
            string result = authBLL.GetDataAuthority(user.UserId, Request.Cookies["currentmoduleId"].Value, jsonData);
            return result;
        }
        [HttpPost]
        [AjaxOnly]
        [HandlerMonitor(5, "获取当前用户对当前模块的功能权限")]
        public string GetOperAuthority(string enCode)
        {
            //AuthorizeBLL authBLL = new AuthorizeBLL();
            //return authBLL.GetOperAuthority(OperatorProvider.Provider.Current(), Request.Cookies["currentmoduleId"].Value, enCode);

            //20190515 sx 从双控对接按钮和数据权限
            string userId = OperatorProvider.Provider.Current().UserId;
            string moduleId = Request.Cookies["currentmoduleId"].Value;

            string res = HttpMethods.HttpGet(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "BaseData", "getUserPermission") + "?" + string.Format("userId={0}&moduleId={1}", userId, moduleId));
            var ret = JsonConvert.DeserializeObject<RetData>(res);
            if (ret != null && ret.code == 0 && ret.data != null)
            {
                //成功
                return ret.data.operScope;
            }
            return "";
        }
        [HttpPost]
        [AjaxOnly]
        [HandlerMonitor(5, "获取当前用户对当前模块的功能权限")]
        public JsonResult GetAuthorityBotton()
        {
            //AuthorizeBLL authBLL = new AuthorizeBLL();
            //return authBLL.GetOperAuthority(OperatorProvider.Provider.Current(), Request.Cookies["currentmoduleId"].Value, enCode);

            //20190515 sx 从双控对接按钮和数据权限
            try
            {
                string userId = OperatorProvider.Provider.Current().UserId;
                string moduleId = Request.Cookies["currentmoduleId"].Value;

                string res = HttpMethods.HttpGet(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "BaseData", "getUserPermission") + "?" + string.Format("userId={0}&moduleId={1}", userId, moduleId));
                var ret = JsonConvert.DeserializeObject<RetData>(res);
                return Json(new { ret.data.operScope, ret.data.dataScope,id1=userId,id2=moduleId });
                //string operScote = JsonConvert.SerializeObject(ret.data.operScope);
                //string dataScope = JsonConvert.SerializeObject(ret.data.dataScope );
                //return new { operScote= operScote, dataScope= dataScope };
                //string s = "{\"operScope\":[{\"encode\":\"add\",\"fullname\":\"新增\",\"faimage\":\"fa fa-plus\",\"actionname\":\"fn$add\"},{\"encode\":\"export\",\"fullname\":\"导出\",\"faimage\":\"fa fa-download\",\"actionname\":\"export\"}]\",\"dataScope\":[{\"authorizetype\":4.0,\"encode\":\"edit\",\"faimage\":\"fa fa-pencil-square-o\",\"actionname\":\"fn$edit\",\"fullname\":\"修改\"},{\"authorizetype\":4.0,\"encode\":\"delete\",\"faimage\":\"fa fa-trash-o\",\"actionname\":\"del\",\"fullname\":\"删除\"},{\"authorizetype\":4.0,\"encode\":\"dafen\",\"faimage\":\"fa fa-calculator\",\"actionname\":\"fn$score\",\"fullname\":\"打分\"},{\"authorizetype\":4.0,\"encode\":\"fabu\",\"faimage\":\"fa fa-rocket\",\"actionname\":\"fn$publish\",\"fullname\":\"发布\"}]}";
                //return s;
            }
            catch (Exception ex)
            {
                return Json("");
            }
       
        }
        


        /// <summary>
        /// 用户数据返回结构
        /// </summary>
        public class RetData
        {
            public int code { get; set; }
            public string message { get; set; }
            public UserPermissionData data { get; set; }
        }
        /// <summary>
        /// 用户权限结构
        /// </summary>
        public class UserPermissionData
        {
            /// <summary>
            /// 数据权限
            /// </summary>
            public string dataScope { get; set; }
            /// <summary>
            /// 功能权限
            /// </summary>
            public string operScope { get; set; }

        }

        public class dataScope
        {
            public int authorizetype { get; set; }
            public string encode { get; set; }
            public string fullname { get; set; }
        }
        public class operScope
        {
            public string encode { get; set; }
            public string fullname { get; set; }
        }



        #endregion
    }
}
