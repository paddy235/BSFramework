using BSFramework.Application.Busines.AuthorizeManage;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.AuthorizeManage;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Web.Areas.Works.Views.EduTrain;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.BaseManage.Controllers
{
    /// <summary>
    /// 描 述：用户管理
    /// </summary>
    public class UserController : MvcControllerBase
    {
        private UserBLL userBLL = new UserBLL();
        //private UserCache userCache = new UserCache();
        private OrganizeBLL organizeBLL = new OrganizeBLL();
        //private OrganizeCache organizeCache = new OrganizeCache();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        //private DepartmentCache departmentCache = new DepartmentCache();
        private ModuleFormInstanceBLL moduleFormInstanceBll = new ModuleFormInstanceBLL();

        #region 视图功能
        /// <summary>
        /// 用户管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = departmentBLL.GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }
        /// <summary>
        /// 用户表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 查看详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Detail(string keyValue)
        {
            string newKey = "!2#3@1YV";
            string newIV = "A~we!S6d";
            Security sec = new Security(newKey, newIV);
            //var current = OperatorProvider.Provider.Current();
            //var userAccount = (new UserBLL()).GetTrainUser(current.UserId);
            //string userAccount = Config.GetValue("userAccount");  //学员对应的培训平台账号
            var current = (new UserBLL()).GetEntity(keyValue);
            string valCode = sec.Encrypt(current == null ? string.Empty : current.Account, newKey, newIV);
            ViewData["valCode"] = valCode;
            ViewData["EduPageUrl"] = Config.GetValue("EduPageUrl");
            return View();
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult RevisePassword()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回机构+部门+用户树形Json</returns>
        [HttpGet]
        //[HandlerMonitor(3, "查询机构+部门+用户树形Json数据!")]
        public ActionResult GetTreeJson(string keyword)
        {
            var organizedata = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
            if (organizedata == null)
            {
                organizedata = organizeBLL.GetList();
            }
            var departmentdata = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
            if (departmentdata == null)
            {
                departmentdata = departmentBLL.GetList();
            }
            var userdata = CacheFactory.Cache().GetCache<IEnumerable<UserEntity>>(userBLL.cacheKey); //userCache.GetList();
            if (userdata == null)
            {
                userdata = userBLL.GetList();
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                    //if (hasChildren == false)
                    //{
                    //    continue;
                    //}
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                #region 部门
                TreeEntity tree = new TreeEntity();
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
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Department";
                treeList.Add(tree);
                #endregion
            }
            foreach (UserEntity item in userdata)
            {
                #region 用户
                TreeEntity tree = new TreeEntity();
                tree.id = item.UserId;
                tree.text = item.RealName;
                tree.value = item.Account;
                tree.parentId = item.DepartmentId;
                tree.title = item.RealName + "（" + item.Account + "）";
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.Attribute = "Sort";
                tree.AttributeValue = "User";
                tree.img = "fa fa-user";
                treeList.Add(tree);
                #endregion
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                treeList = treeList.TreeWhere(t => t.text.Contains(keyword), "id", "parentId");
            }
            return Content(treeList.TreeToJson());
        }

        #region 获取机构部门组织树菜单
        /// <summary>
        /// 获取机构部门组织树菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDepartTreeJson()
        {
            //暂时取所有，后期用户权限开发完成，数据将通过权限下来进行展示 
            var organizedata = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
            if (organizedata == null)
            {
                organizedata = organizeBLL.GetList();
            }
            var departmentdata = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
            if (departmentdata == null)
            {
                departmentdata = departmentBLL.GetList();
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                #region 部门
                TreeEntity tree = new TreeEntity();
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
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Department";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            return Content(treeList.TreeToJson());
        }
        #endregion
        [HttpGet]
        public ActionResult CheckTel(string tel, string keyValue)
        {
            var data = userBLL.GetList();
            bool hasUser = data.Count(t => t.Mobile == tel && t.UserId != keyValue) == 0 ? false : true;
            if (hasUser == true)
            {
                return Success("1");
            }
            else
            {
                return Success("0");
            }
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="departmentId">部门Id</param>
        /// <returns>返回用户列表Json</returns>
        [HttpGet]
        //[HandlerMonitor(3, "根据部门查询用户列表!")]
        public ActionResult GetListJson(string departmentId)
        {
            var userdata = CacheFactory.Cache().GetCache<IEnumerable<UserEntity>>(userBLL.cacheKey); //userCache.GetList();
            if (userdata == null)
            {
                userdata = userBLL.GetList();
            }

            var data = userdata.Where(x => x.DepartmentId == departmentId).ToList();
            return Content(data.ToJson());
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        //[HandlerMonitor(3, "分页查询用户信息!")]
        public JsonResult GetPageListJson(string name, string account, string deptid, int rows, int page)
        {
            var depts = new DepartmentBLL().GetSubDepartments(deptid, null);

            var total = 0;
            var data = userBLL.GetList(name, account, depts.Select(x => x.DepartmentId).ToArray(), rows, page, out total);


            return Json(new { rows = data, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);


            //pagination.p_kid = "USERID";
            //pagination.p_fields = @"senddeptid,REALNAME,MOBILE,OrganizeName,ORGANIZEID,DEPTNAME,departmentid,DEPARTMENTCODE,DUTYNAME,POSTNAME,ROLENAME,ROLEID,MANAGER,ENABLEDMARK,ENCODE,ACCOUNT,NICKNAME,HEADICON,GENDER,EMAIL,OrganizeCode,createdate";
            //pagination.p_tablename = @"(select USERID,t2.senddeptid,t.REALNAME,t.MOBILE,t1.fullname as OrganizeName,t.ORGANIZEID,t2.fullname as DEPTNAME,t.departmentid,t2.encode as DEPARTMENTCODE,t.DUTYNAME,t.POSTNAME,t.ROLENAME,t.ROLEID,t.MANAGER,t.ENABLEDMARK,t.ENCODE,t.ACCOUNT,t.NICKNAME,t.HEADICON,t.GENDER,t.EMAIL,t1.encode as OrganizeCode,t.createdate from base_user t left join base_organize t1 on t.organizeid = t1.organizeid left join base_department t2 on t.departmentid=t2.departmentid) tb";
            //pagination.conditionJson = "Account!='System'";
            //var watch = CommonHelper.TimerStart();
            //var data = userBLL.GetPageList(pagination, queryJson);
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

        public ActionResult GetLegalsJson(Pagination pagination, string userid)
        {
            LllegalBLL lbll = new LllegalBLL();
            pagination.p_kid = "ID";
            pagination.p_fields = "LllegalNumber,LllegalType,LllegalTime,LllegalLevel,LllegalPerson,LllegalPersonId,LllegalTeam,LllegalTeamId,LllegalDepart,LllegalDepartCode,LllegalDescribe,RegisterPerson,RegisterPersonId,LllegalAddress,REMARK";
            pagination.p_tablename = "wg_lllegalregister t";
            pagination.conditionJson = "LllegalPersonId like '%" + userid + "%'";
            pagination.sidx = "LllegalTime";
            var watch = CommonHelper.TimerStart();
            var data = lbll.GetLegalsList(pagination);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 用户实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        //[HandlerMonitor(3, "查询用户对象信息!")]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = userBLL.GetEntity(keyValue);
            return Content(data.ToJson());
        }
        [HttpGet]
        public ActionResult GetUserInfo(string keyValue)
        {
            var data = userBLL.GetEntity(keyValue);
            return Content(data.ToJson());
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 账户不能重复
        /// </summary>
        /// <param name="Account">账户值</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistAccount(string Account, string keyValue)
        {
            bool IsOk = userBLL.ExistAccount(Account, keyValue);
            return Content(IsOk.ToString());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除用户信息")]
        public ActionResult RemoveForm(string keyValue)
        {
            if (keyValue == "System")
            {
                throw new Exception("当前账户不能删除");
            }
            Erchtms e = new Erchtms();
            UserEntity entity = userBLL.GetEntity(keyValue);
            e.ErchtmsSynchronoous("DeleteUser", entity, entity.Account);
            userBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)用户信息")]
        public ActionResult SaveForm(string keyValue, string strUserEntity, string FormInstanceId, string strModuleFormInstanceEntity)
        {
            UserEntity userEntity = strUserEntity.ToObject<UserEntity>();
            ModuleFormInstanceEntity moduleFormInstanceEntity = strModuleFormInstanceEntity.ToObject<ModuleFormInstanceEntity>();
            userEntity.CreateUserId = OperatorProvider.Provider.Current().UserId;
            userEntity.CreateUserName = OperatorProvider.Provider.Current().UserName;
            userEntity.Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower();
            userEntity.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(userEntity.Password, 32).ToLower(), userEntity.Secretkey).ToLower(), 32).ToLower();

            string objectId = userBLL.SaveForm(keyValue, userEntity);
            moduleFormInstanceEntity.ObjectId = objectId;
            moduleFormInstanceBll.SaveEntity(FormInstanceId, moduleFormInstanceEntity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 修改用户基本信息
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(7, "修改用户信息")]
        public ActionResult UpdateForm(string keyValue, string strUserEntity)
        {
            UserEntity userEntity = strUserEntity.ToObject<UserEntity>();

            string objectId = userBLL.UpdateUserInfo(keyValue, userEntity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 保存重置修改密码
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="Password">新密码</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(7, "修改用户密码信息")]
        public ActionResult SaveRevisePassword(string keyValue, string Password)
        {
            if (keyValue == "System")
            {
                throw new Exception("当前账户不能重置密码");
            }
            userBLL.RevisePassword(keyValue, Password);
            return Success("密码修改成功，请牢记新密码。");
        }
        /// <summary>
        /// 禁用账户
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(7, "设置当前用户账户不可用")]
        public ActionResult DisabledAccount(string keyValue)
        {
            if (keyValue == "System")
            {
                throw new Exception("当前账户不禁用");
            }
            userBLL.UpdateState(keyValue, 0);
            return Success("账户禁用成功。");
        }
        /// <summary>
        /// 启用账户
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(7, "设置当前用户账户可用")]
        public ActionResult EnabledAccount(string keyValue)
        {
            userBLL.UpdateState(keyValue, 1);
            return Success("账户启用成功。");
        }
        #endregion

        #region 数据导出
        /// <summary>
        /// 导出用户列表
        /// </summary>
        /// <returns></returns>
        [HandlerMonitor(0, "导出用户数据")]
        public ActionResult ExportUserList(string condition, string keyword, string code, string isOrg)
        {
            userBLL.GetExportList(condition, keyword, code, isOrg);
            return Success("导出成功。");
        }
        #endregion

        public ViewResult Select()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.deptid = user.DeptId;
            return View();
        }

        #region 获取机构部门组织树菜单
        /// <summary>
        /// 获取机构部门组织树菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDeptTreeJson()
        {
            //暂时取所有，后期用户权限开发完成，数据将通过权限下来进行展示 
            Operator user = OperatorProvider.Provider.Current();
            List<OrganizeEntity> organizedata = new List<OrganizeEntity>();
            List<DepartmentEntity> departmentdata = new List<DepartmentEntity>();
            string roleNames = user.RoleName;
            if (user.IsSystem)
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                organizedata = organizedata1.ToList();
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }
                departmentdata = departmentdata1.ToList();
            }
            else
            {
                var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                if (organizedata1 == null)
                {
                    organizedata1 = organizeBLL.GetList();
                }
                var departmentdata1 = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey);//departmentCache.GetList();
                if (departmentdata1 == null)
                {
                    departmentdata1 = departmentBLL.GetList();
                }

                organizedata = organizedata1.Where(t => t.OrganizeId == user.OrganizeId).OrderByDescending(x => x.CreateDate).ToList();

                //if (roleNames.Contains("公司级用户") || roleNames.Contains("厂级部门用户"))
                //{
                departmentdata = departmentdata1.OrderBy(x => x.SortCode).ToList();
                //}
                //else
                //{
                //    departmentdata = departmentCache.GetList(user.OrganizeId).Where(t => t.EnCode.Contains(user.DeptCode) || t.Description == "外包工程承包商").OrderBy(x => x.SortCode).ToList();
                //}
            }
            var treeList = new List<TreeEntity>();
            foreach (OrganizeEntity item in organizedata)
            {
                #region 机构
                //if (existAuthorizeData.Count(t => t.ResourceId == item.OrganizeId) == 0) continue;
                TreeEntity tree = new TreeEntity();
                bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
                if (hasChildren == false)
                {
                    hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
                }
                tree.id = item.OrganizeId;
                tree.text = item.FullName;
                tree.value = item.OrganizeId;
                tree.parentId = item.ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Organize";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            foreach (DepartmentEntity item in departmentdata)
            {
                #region 部门
                TreeEntity tree = new TreeEntity();
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
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = true;
                tree.Attribute = "Sort";
                tree.AttributeValue = (item.Nature == "分包商" || item.Nature == "承包商" || item.Description == "外包工程承包商") && !(roleNames.Contains("公司级用户") || roleNames.Contains("厂级部门用户")) ? "Contract" : "Department";
                tree.AttributeA = "EnCode";
                tree.AttributeValueA = item.EnCode;
                treeList.Add(tree);
                #endregion
            }
            return Content(treeList.TreeToJson());
        }
        #endregion

        /// <summary>
        /// 选择用户页面使用（不判断权限）
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>


        public JsonResult GetList(int rows, int page, string deptid, string key, string value, bool deep)
        {
            var list = deptid.Split(',');
            if (deep)
            {
                var depts = new DepartmentBLL().GetSubDepartments(deptid.Split(','));
                list = depts.Select(x => x.DepartmentId).ToArray();
            }
            var total = 0;
            var data = userBLL.GetList(list, rows, page, out total, key, value);
            return Json(new { rows = data, records = total, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 人脸
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ViewResult ViewFace(string id)
        {
            var files = new FileInfoBLL().GetFilesByRecIdNew(id);
            var file = files.FirstOrDefault(x => x.Description == "人脸");
            if (file == null) ViewBag.src = null;
            else ViewBag.src = Url.Content(file.FilePath);

            return View();
        }
    }
}
