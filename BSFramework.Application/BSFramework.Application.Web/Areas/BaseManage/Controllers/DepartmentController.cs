using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.LllegalManage;
using BSFramework.Application.Busines.PeopleManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Web.Areas.BaseManage.Models;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Cache.Factory;
using BSFramework.Entity.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
namespace BSFramework.Application.Web.Areas.BaseManage.Controllers
{
    /// <summary>
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentController : MvcControllerBase
    {
        //private OrganizeCache organizeCache = new OrganizeCache();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        //private DepartmentCache departmentCache = new DepartmentCache();

        #region 视图功能
        /// <summary>
        /// 部门管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult More()
        {
            return View();
        }
        public ActionResult MoreOverView()
        {
            return View();
        }
        /// <summary>
        /// 部门表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 选择部门
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Select()
        {
            return View();
        }
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult SelectBZ()
        {
            return View();
        }
        /// <summary>
        /// 部门详情
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Detail(string keyValue)
        {
            return View();
        }
        /// <summary>
        /// 班组详情
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult BzDetail(string keyValue)
        {
            return View();
        }
        #endregion

        #region 获取数据
        public ActionResult GetMore()
        {
            var user = OperatorProvider.Provider.Current();
            LllegalBLL lbll = new LllegalBLL();
            var data = lbll.GetMore(user.DeptId);
            var JsonData = new
            {
                rows = data
            };
            return Content(JsonData.ToJson());
        }

        public ActionResult GetMore1()
        {
            WorkmeetingBLL workmeetingbll = new WorkmeetingBLL();
            var user = OperatorProvider.Provider.Current();
            var data = workmeetingbll.GetOverView(user.DeptId);
            var result = data.Select(x => new WorkmeetingEntity() { MeetingId = x.FullName, MeetingPerson = ((decimal)x.Layer.Value / x.SortCode.Value).ToString("p") }).ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Bz");
            dt.Columns.Add("Num");
            foreach (WorkmeetingEntity w in result)
            {
                DataRow row = dt.NewRow();
                row[0] = w.MeetingId;
                row[1] = w.MeetingPerson;
                dt.Rows.Add(row);
            }

            var JsonData = new
            {
                rows = dt
            };
            return Content(JsonData.ToJson());
        }
        public ActionResult GetLegalsJson(Pagination pagination, string bzid)
        {
            LllegalBLL lbll = new LllegalBLL();
            pagination.p_kid = "ID";
            pagination.p_fields = "LllegalNumber,LllegalType,LllegalTime,LllegalLevel,LllegalPerson,LllegalPersonId,LllegalTeam,LllegalTeamId,LllegalDepart,LllegalDepartCode,LllegalDescribe,RegisterPerson,RegisterPersonId,LllegalAddress,REMARK";
            pagination.p_tablename = "wg_lllegalregister t";
            pagination.conditionJson = "LllegalTeamId ='" + bzid + "'";
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
        public ActionResult GetPeopleJson(Pagination pagination, string bzid)
        {
            PeopleBLL pbll = new PeopleBLL();
            pagination.p_kid = "ID";
            pagination.p_fields = "Name,LinkWay,Quarters,Planer,bzid";
            pagination.p_tablename = "wg_people t";
            pagination.conditionJson = "bzid ='" + bzid + "' and quarters !='其他成员'";
            pagination.sidx = "Planer";
            var watch = CommonHelper.TimerStart();
            var data = pbll.GetPeopleJson(pagination);
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
        /// 部门列表 
        /// </summary>
        /// <param name="organizeId">公司Id</param>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetTreeJson(string organizeId, string keyword)
        {
            var cachedata = CacheFactory.Cache().GetCache<IEnumerable<DepartmentEntity>>(departmentBLL.cacheKey); //departmentCache.GetList(roleEntity.OrganizeId);
            if (cachedata == null)
            {
                cachedata = departmentBLL.GetList();
            }
            var data = cachedata.Where(x => x.OrganizeId == organizeId).ToList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.FullName.Contains(keyword), "DepartmentId");
            }
            var treeList = new List<TreeEntity>();
            foreach (DepartmentEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = data.Count(t => t.ParentId == item.DepartmentId) == 0 ? false : true;
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 部门列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回机构+部门树形Json</returns>
        public ActionResult GetOrganizeTreeJson(string keyword)
        {
            var organizedata = new OrganizeBLL().GetList();//organizeCache.GetList();
            var departmentdata = departmentBLL.GetList();
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
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.Attribute = "Sort";
                tree.AttributeValue = "Department";
                treeList.Add(tree);
                #endregion
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                treeList = treeList.TreeWhere(t => t.text.Contains(keyword), "id", "parentId");
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 部门列表 
        /// </summary>
        /// <param name="condition">查询条件</param>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形列表Json</returns>
        [HttpGet]
        public ActionResult GetTreeListJson(string condition, string keyword)
        {
            //var organizedata = new OrganizeBLL().GetList();//organizeCache.GetList();
            var departmentdata = departmentBLL.GetList().ToList();
            if (!string.IsNullOrEmpty(condition) && !string.IsNullOrEmpty(keyword))
            {
                #region 多条件查询
                switch (condition)
                {
                    case "FullName":    //部门名称
                        departmentdata = departmentdata.TreeWhere(t => t.FullName.Contains(keyword), "DepartmentId");
                        break;
                    case "EnCode":      //部门编号
                        departmentdata = departmentdata.TreeWhere(t => t.EnCode.Contains(keyword), "DepartmentId");
                        break;
                    case "ShortName":   //部门简称
                        departmentdata = departmentdata.TreeWhere(t => t.ShortName.Contains(keyword), "DepartmentId");
                        break;
                    case "Manager":     //负责人
                        departmentdata = departmentdata.TreeWhere(t => t.Manager.Contains(keyword), "DepartmentId");
                        break;
                    case "OuterPhone":  //电话号
                        departmentdata = departmentdata.TreeWhere(t => t.OuterPhone.Contains(keyword), "DepartmentId");
                        break;
                    case "InnerPhone":  //分机号
                        departmentdata = departmentdata.TreeWhere(t => t.Manager.Contains(keyword), "DepartmentId");
                        break;
                    default:
                        break;
                }
                #endregion
            }
            var treeList = new List<TreeGridEntity>();
            //foreach (OrganizeEntity item in organizedata)
            //{
            //    TreeGridEntity tree = new TreeGridEntity();
            //    bool hasChildren = organizedata.Count(t => t.ParentId == item.OrganizeId) == 0 ? false : true;
            //    if (hasChildren == false)
            //    {
            //        hasChildren = departmentdata.Count(t => t.OrganizeId == item.OrganizeId) == 0 ? false : true;
            //        //if (hasChildren == false)
            //        //{
            //        //    continue;
            //        //}
            //    }
            //    tree.id = item.OrganizeId;
            //    tree.hasChildren = hasChildren;
            //    tree.parentId = item.ParentId;
            //    tree.expanded = true;
            //    //item.EnCode = "";item.ShortName = ""; item.Nature = "";item.Manager = "";item.OuterPhone = ""; item.InnerPhone = ""; item.Description = "";
            //    string itemJson = item.ToJson();
            //    itemJson = itemJson.Insert(1, "\"DepartmentId\":\"" + item.OrganizeId + "\",");

            //    itemJson = itemJson.Insert(1, "\"Sort\":\"Organize\",");
            //    tree.entityJson = itemJson;
            //    treeList.Add(tree);
            //}
            foreach (DepartmentEntity item in departmentdata)
            {
                TreeGridEntity tree = new TreeGridEntity();
                bool hasChildren = departmentdata.Count(t => t.ParentId == item.DepartmentId) == 0 ? false : true;
                tree.id = item.DepartmentId;
                //if (item.ParentId == "0")
                //{
                //    tree.parentId = item.OrganizeId;
                //}
                //else
                //{
                tree.parentId = item.ParentId;
                //}
                tree.expanded = true;
                tree.hasChildren = hasChildren;
                string itemJson = item.ToJson();
                itemJson = itemJson.Insert(1, "\"Sort\":\"Department\",");
                tree.entityJson = itemJson;
                treeList.Add(tree);
            }
            return Content(treeList.TreeJson());
        }
        /// <summary>
        /// 部门实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public JsonResult GetFormJson(string keyValue)
        {
            var data = departmentBLL.GetEntity(keyValue);
            return Success(new AjaxResult { type = ResultType.success, resultdata = data });
        }

        /// <summary>
        /// 获取当前电厂下所有的班组
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public ActionResult GetDeptTreeJson(string Ids, int checkMode = 0, int mode = 0)
        {
            OrganizeBLL organizeBLL = new OrganizeBLL();
            Operator user = OperatorProvider.Provider.Current();
            List<OrganizeEntity> organizedata = new List<OrganizeEntity>();
            List<DepartmentEntity> departmentdata = new List<DepartmentEntity>();
            string roleNames = user.RoleName;
            if (string.IsNullOrEmpty(Ids)) Ids = departmentBLL.GetRootDepartment().DepartmentId;
            var depts = Ids.Split(',');
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
                //var organizedata1 = CacheFactory.Cache().GetCache<IEnumerable<OrganizeEntity>>(organizeBLL.cacheKey);//organizeCache.GetList();
                //if (organizedata1 == null)
                //{
                //    organizedata1 = organizeBLL.GetList();
                //}
                //organizedata = organizedata1.ToList();
                if (mode == 1)
                    departmentdata = new DepartmentBLL().GetDepartments(depts);
                else
                    departmentdata = new DepartmentBLL().GetSubDepartments(depts);
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
                if (item.Nature == "班组")
                {
                    tree.Attribute = "Code";
                    tree.AttributeValue = item.EnCode;
                }
                if (item.Nature == "班组" || item.Nature == "部门")
                {
                    tree.showcheck = checkMode == 0 ? false : true;
                }
                treeList.Add(tree);
                #endregion
            }
            return Content(treeList.TreeToJson(string.IsNullOrEmpty(Ids) ? "0" : string.Join(",", treeList.FindAll(x => depts.Contains(x.id)).Select(x => x.parentId))));
        }
        #endregion


        public JsonResult GetDepartments(string deptid, string category, string checkMode)
        {
            var user = OperatorProvider.Provider.Current();
            var root = departmentBLL.GetAuthorizationDepartment(string.IsNullOrEmpty(deptid) ? user.DeptId : deptid);

            var depts = departmentBLL.GetSubDepartments(root.DepartmentId, category);


            //GetSubDepartments 获取category类型下的节点
            //if (string.IsNullOrEmpty(Ids) || Ids == "0")
            //{
            //    depts = departmentBLL.GetSubDepartments(rootdpet.DepartmentId, category);
            //    Ids = rootdpet.DepartmentId;
            //}
            //else
            //{
            //    var dept = departmentBLL.GetEntity(Ids);
            //    if (dept.ParentId != "0")
            //    {


            //        if (dept.IsSpecial) //特殊部门
            //        {
            //            var parroot = departmentBLL.GetRootDepartment();
            //            depts = departmentBLL.GetSubDepartments(parroot.DepartmentId, category);

            //        }
            //        else
            //        {
            //            depts = departmentBLL.GetSubDepartments(Ids, category);

            //        }
            //    }
            //    else
            //    {

            //    }
            //}
            //截取树型
            //var categoryStr = category.Split(',');
            //if (categoryStr.Count() > 0)
            //{
            //    //按第一位节点作为父节点
            //    int j = 0;
            //    for (int i = 0; i < categoryStr.Count(); i++)
            //    {
            //        if (depts.Where(x => x.Nature == categoryStr[i]).Count() > 0)
            //        {
            //            j = i;
            //            break;
            //        }
            //    }

            //    foreach (var item in depts)
            //    {
            //        //tree父节点parentid=0
            //        if (item.Nature == categoryStr[j])
            //        {
            //            //var ck = depts.Where(x => item.DepartmentId ==x.ParentId );
            //            //if (ck.Count() == 0)
            //            //{
            //            item.ParentId = "0";
            //            //}
            //            //添加父节点序列化treeitems.TreeToJson(Ids)
            //            //if (string.IsNullOrEmpty(Ids))
            //            //{
            //            //    Ids = item.DepartmentId;
            //            //}
            //            //else
            //            //{
            //            //    Ids += "," + item.DepartmentId;
            //            //}
            //        }
            //    }
            //}
            //var treeitems = depts.Select(x => new TreeEntity() { AttributeA = "code", AttributeValueA = x.EnCode, id = x.DepartmentId, text = x.FullName, value = x.DepartmentId, parentId = x.ParentId, isexpand = true, complete = true, hasChildren = depts.Count(y => y.ParentId == x.DepartmentId) > 0, showcheck = true, Attribute = "Nature", AttributeValue = x.Nature }).ToList();
            //return Content(treeitems.TreeToJson(Ids));

            var data = this.BuildTree(depts, root.DepartmentId, checkMode == "multiple");

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        private List<TreeModel> BuildTree(List<DepartmentEntity> depts, string root, bool showCheck)
        {
            return depts.Where(x => x.DepartmentId == root).Select(x => new TreeModel
            {
                id = x.DepartmentId,
                value = x.DepartmentId,
                text = x.FullName,
                Code = x.EnCode,
                isexpand = depts.Count(y => y.ParentId == x.DepartmentId) > 0,
                hasChildren = depts.Count(y => y.ParentId == x.DepartmentId) > 0,
                ChildNodes = GetChildren(depts, x.DepartmentId, showCheck),
                showcheck = showCheck
            }).ToList();
        }

        /// <summary>
        /// 保存部门code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetDepartmentsBZ(string Ids, string checkMode)
        {
            var rootdpet = departmentBLL.GetRootDepartment();
            var depts = new List<DepartmentEntity>();
            if (string.IsNullOrEmpty(Ids) || Ids == "0")
            {
                depts = departmentBLL.GetSubDepartments(rootdpet.DepartmentId, null);

            }

            else
            {
                var dept = departmentBLL.GetEntity(Ids);
                if (dept.IsSpecial) //特殊部门
                {
                    depts = departmentBLL.GetSubDepartments(dept.ParentId, null);
                    var root = depts.FirstOrDefault(x => x.DepartmentId == dept.ParentId);
                    root.ParentId = "0";
                }
                else
                {
                    depts = departmentBLL.GetSubDepartments(Ids, null);
                    var root = depts.FirstOrDefault(x => x.DepartmentId == Ids);
                    root.ParentId = "0";
                }

            }

            var treeitems = depts.Select(x => new TreeEntity() { Code = x.EnCode, id = x.DepartmentId, text = x.FullName, value = x.EnCode, parentId = x.ParentId, isexpand = true, complete = true, hasChildren = depts.Count(y => y.ParentId == x.DepartmentId) > 0, showcheck = checkMode == "0" ? false : true, Attribute = "Nature", AttributeValue = x.Nature }).ToList();
            return Content(treeitems.TreeToJson());
        }
        #region 验证数据
        /// <summary>
        /// 部门编号不能重复
        /// </summary>
        /// <param name="enCode">编号</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistEnCode(string EnCode, string keyValue)
        {
            bool IsOk = departmentBLL.ExistEnCode(EnCode, keyValue);
            return Content(IsOk.ToString());
        }
        /// <summary>
        /// 部门名称不能重复
        /// </summary>
        /// <param name="FullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistFullName(string FullName, string keyValue)
        {
            bool IsOk = departmentBLL.ExistFullName(FullName, keyValue);
            return Content(IsOk.ToString());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除部门信息")]
        public ActionResult RemoveForm(string keyValue)
        {
            Erchtms e = new Erchtms();
            DepartmentEntity entity = departmentBLL.GetEntity(keyValue);
            e.ErchtmsSynchronoous("DeleteDept", entity, "");

            if (departmentBLL.RemoveForm(keyValue))
            {
                return Success("删除成功。");  //删除成功
            }
            else
            {
                return Success("删除失败。");
            }
        }
        /// <summary>
        /// 保存部门表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">部门实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)部门信息")]
        public ActionResult SaveForm(string keyValue, DepartmentEntity departmentEntity)
        {
            departmentBLL.SaveForm(keyValue, departmentEntity);
            Erchtms e = new Erchtms();
            e.ErchtmsSynchronoous("SaveDept", departmentEntity, "");
            return Success("操作成功。");
        }
        #endregion

        public ActionResult DeptSelect()
        {
            return View();
        }

        public JsonResult GetData(string id)
        {
            var user = OperatorProvider.Provider.Current();
            var select = this.Request.QueryString.Get("select");
            var category = this.Request.QueryString.Get("category");

            if (string.IsNullOrEmpty(id))
            {
                var rootdpet = departmentBLL.GetAuthorizationDepartment(user.DeptId);
                id = rootdpet.DepartmentId;
            }
            var depts = departmentBLL.GetSubDepartments(id, category);
            //可以根据部门类型 获取不同树型
            var makeDepts = depts.Where(x => x.DepartmentId == id);
            if (makeDepts.Count() == 0)
            {
                if (!string.IsNullOrEmpty(category))
                {
                    var one = category.Split(',');
                    makeDepts = depts.Where(x => x.Nature == one[0]);
                }

            }

            var data = makeDepts.Select(x => new TreeModel() { id = x.DepartmentId, Code = x.EnCode, value = x.DepartmentId, text = x.FullName, hasChildren = depts.Count(y => y.ParentId == x.DepartmentId) > 0, isexpand = depts.Count(y => y.ParentId == x.DepartmentId) > 0, ChildNodes = GetChildren(depts, x.DepartmentId, select == "multi"), showcheck = select == "multi" });


            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 递归树
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<TreeModel> GetChildren(List<DepartmentEntity> data, string id, bool showcheck)
        {
            return data.Where(x => x.ParentId == id).Select(x => new TreeModel
            {
                id = x.DepartmentId,
                value = x.DepartmentId,
                Code = x.EnCode,
                text = x.FullName,
                isexpand = data.Count(y => y.ParentId == x.DepartmentId) > 0,
                hasChildren = data.Count(y => y.ParentId == x.DepartmentId) > 0,
                ChildNodes = GetChildren(data, x.DepartmentId, showcheck),
                showcheck = showcheck
            }).ToList();
        }

        [HttpGet]
        public JsonResult GetDepartmentTree(string deptid, bool showCheck = false)
        {
            var bll = new DepartmentBLL();
            if (string.IsNullOrEmpty(deptid))
            {
                var rootdpet = departmentBLL.GetRootDepartment();
                deptid = rootdpet.DepartmentId;
            }
            var depts = bll.GetDepartments(deptid);

            var root = depts.Find(x => x.DepartmentId == deptid);
            var pdept = depts.Find(x => x.DepartmentId == root.ParentId);
            if (pdept != null) root = depts.Find(x => x.ParentId == "0");
            depts = depts.OrderBy(x => x.EnCode).ToList();
            var tree = this.GetTree(depts, root, showCheck);
            var rootitem = new TreeModel(showCheck) { id = root.DepartmentId, value = root.DepartmentId, text = root.FullName, ChildNodes = this.GetTree(depts, root, showCheck), hasChildren = depts.Count(y => y.ParentId == root.DepartmentId) > 0, isexpand = true, Code = root.EnCode };
            var treeitems = new List<TreeModel>() { rootitem };
            return Json(treeitems, JsonRequestBehavior.AllowGet);
        }

        private List<TreeModel> GetTree(List<DepartmentEntity> depts, DepartmentEntity dept, bool showCheck = false)
        {
            var items = depts.Where(x => x.ParentId == dept.DepartmentId).OrderBy(x => x.EnCode);
            return items.Select(x => new TreeModel(showCheck) { id = x.DepartmentId, value = x.DepartmentId, text = x.FullName, hasChildren = depts.Count(y => x.DepartmentId == y.ParentId) > 0, isexpand = true, ChildNodes = this.GetTree(depts, x, showCheck), Code = x.EnCode }).ToList();
        }

        public JsonResult GetTreeData()
        {
            var root = departmentBLL.GetRootDepartment();
            var depts = departmentBLL.GetSubDepartments(root.DepartmentId, null);

            return Json(depts, JsonRequestBehavior.AllowGet);
        }
    }
}
