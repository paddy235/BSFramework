using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BSFramework.Application.Busines;
using ThoughtWorks.QRCode.Codec;
using BSFramework.Application.Entity;
using BSFramework.Entity.WorkMeeting;
using System.Text.RegularExpressions;
using BSFramework.Application.Busines.WorkMeeting;
using BSFramework.Application.Entity.BaseManage;
using Newtonsoft.Json;
using BSFramework.Application.Entity.WorkMeeting;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 排班
    /// </summary>
    public class PaibanController : MvcControllerBase
    {
        private WorksetupBll sysBll = new WorksetupBll();
        private WorkSettingBLL workset = new WorkSettingBLL();
        private DepartmentBLL departmentBLL = new DepartmentBLL();
        private WorkOrderBLL workorder = new WorkOrderBLL();
        private DepartmentBLL dept = new DepartmentBLL();

        #region 视图功能
        /// <summary>
        /// 班次页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ViewResult Banci()
        {
            return View();
        }
        /// <summary>
        /// 班次修改页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        public ViewResult Form()
        {
            return View();
        }
        /// <summary>
        /// 新建班制
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult Select()
        {

            return View();
        }

        /// <summary>
        /// 班次排序
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult WorkOrder()
        {
            return View();
        }

        /// <summary>
        /// 班次排序设置页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult WorkOrderSet()
        {
            return View();
        }
        /// <summary>
        /// 添加班组选择页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ViewResult WorkGroup()
        {

            return View();
        }

        #endregion



        #region 获取数据

        /// <summary>
        /// 班制 
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetTreeJson()
        {

            var data = sysBll.GetList().ToList();//organizeCache.GetList().ToList();

            var treeList = new List<TreeEntity>();
            foreach (WorkSetTypeEntity item in data)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.Id;
                tree.text = item.SystemName;
                tree.value = item.Id;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                treeList.Add(tree);
            }

            return Content(treeList.TreeToJson());

        }

        /// <summary>
        /// 班制 
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns>返回树形Json</returns>
        [HttpGet]
        public ActionResult GetUseTreeJson()
        {
            var user = OperatorProvider.Provider.Current();
            var data = sysBll.GetList().ToList();//organizeCache.GetList().ToList();
            var workType = workset.GetList(user.DeptId);
            List<WorkSetTypeEntity> outList = new List<WorkSetTypeEntity>();
            foreach (var item in data)
            {
                var one = workType.FirstOrDefault(row => row.WorkSetupId == item.Id);
                if (one != null)
                {
                    outList.Add(item);
                }
            }

            var treeList = new List<TreeEntity>();
            foreach (WorkSetTypeEntity item in outList)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.Id;
                tree.text = item.SystemName;
                tree.value = item.SystemType;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                treeList.Add(tree);
            }

            return Content(treeList.TreeToJson());

        }


        [HttpGet]
        public ActionResult Form2(int row, int col, DateTime TimeSpan, string TimeStr, string orderId, string context)
        {
            ViewData["TimeStr"] = TimeStr;
            var text = context.Contains("休息") ? "休息" : context;
            ViewData["context"] = text;
            ViewData["WorkOrderId"] = orderId;
            ViewData["UpTime"] = TimeSpan.ToString("yyyy-MM-" + (col - 1) + "");
            return View();
        }

        /// <summary>
        /// 排班列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWorkOrderData(DateTime startTime)
        {
            var user = OperatorProvider.Provider.Current();
            var start = Convert.ToDateTime(startTime.ToString("yyyy-MM-01"));

            var num = Time.GetDaysOfMonth(start.Year, start.Month);
            var end = Convert.ToDateTime(startTime.ToString("yyyy-MM-" + num + ""));
            var deptid = dept.GetAuthorizationDepartment(user.DeptId);
            var dataList = workorder.WorkOrderGet(start, end, deptid.DepartmentId).OrderBy(x => x.deptcode);

            var treeList = new List<TreeGridEntity>();
            foreach (var item in dataList)
            {
                TreeGridEntity tree = new TreeGridEntity();
                tree.id = item.worktimesortid;
                tree.hasChildren = false;
                tree.parentId = "0";
                tree.expanded = true;
                string myTimeStr = string.Empty;
                myTimeStr = item.timedata;
                item.timedata = "";
                string itemJson = item.ToJson();
                var strSp = myTimeStr.Split(',');
                for (int i = 0; i < strSp.Count(); i++)
                {
                    itemJson = itemJson.Insert(1, "\"Time" + (i + 1) + "\":\"" + strSp[i] + "\", ");
                }
                itemJson = itemJson.Insert(1, "\"ParentId\":\"0\",");
                tree.entityJson = itemJson;
                treeList.Add(tree);
            }
            var sss = treeList.TreeJson();
            return Content(treeList.TreeJson());

        }

        /// <summary>
        /// 获取部门列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetDepartmentData()
        {
            var departmentdata = departmentBLL.GetList().ToList();
            departmentdata = departmentdata.Where(row => row.Nature == "部门").ToList();
            var treeList = new List<TreeEntity>();
            foreach (DepartmentEntity item in departmentdata)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 获取部门下班组列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DepartmentGroupData()
        {
            var user = OperatorProvider.Provider.Current();
            var departmentdata = departmentBLL.GetList().ToList();
            var departmentGroupdata = departmentdata.Where(row => row.ParentId == user.DeptId && row.Nature == "班组").ToList();
            if (user.DeptId == "0")
            {
                departmentGroupdata = departmentdata.Where(row => row.Nature == "班组").ToList();
            }
            var treeList = new List<TreeGridEntity>();
            foreach (DepartmentEntity item in departmentGroupdata)
            {
                TreeGridEntity tree = new TreeGridEntity();
                tree.id = item.DepartmentId;
                tree.hasChildren = false;
                tree.parentId = "0";
                tree.expanded = true;
                item.ParentId = "0";
                string itemJson = item.ToJson();
                tree.entityJson = itemJson;
                treeList.Add(tree);
            }
            return Content(treeList.TreeJson());
        }
        /// <summary>
        /// 获取同一批次的班组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult WorkGroupData(string DepId)
        {
            var OrderList = workorder.GetWorkOrderGroup(DepId).ToList();
            var treeList = new List<TreeGridEntity>();
            foreach (WorkGroupSetEntity item in OrderList)
            {
                TreeGridEntity tree = new TreeGridEntity();
                tree.id = item.departmentid;
                tree.hasChildren = false;
                tree.parentId = "0";
                tree.expanded = true;
                string itemJson = item.ToJson();
                itemJson = itemJson.Insert(1, "\"ParentId\":\"0\",");
                itemJson = itemJson.Insert(1, "\"DepartmentId\":\"" + item.departmentid + "\",");
                itemJson = itemJson.Insert(1, "\"FullName\":\"" + item.fullname + "\",");
                tree.entityJson = itemJson;
                treeList.Add(tree);
            }
            return Content(treeList.TreeJson());
        }
        /// <summary>
        /// 获取部门下班组下拉列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DepartmentSelectData()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.DepartMentId = user.DeptId;
            var dept = departmentBLL.GetAuthorizationDepartment(user.DeptId);
            var departmentGroupdata = departmentBLL.GetSubDepartments(dept.DepartmentId, "班组");

            var treeList = new List<TreeEntity>();
            foreach (DepartmentEntity item in departmentGroupdata)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.DepartmentId;
                tree.text = item.FullName;
                tree.value = item.DepartmentId;
                tree.title = item.EnCode;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                treeList.Add(tree);

            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 获取班次列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWorksettingbyOrder(string orderid)
        {
            var user = OperatorProvider.Provider.Current();
            var data = workorder.getSetValue(user.DeptId);
            //var order = workorder.GetWorkOrderday(orderid);
            //var data = workset.GetList(user.DeptId).Where(row => row.WorkSetupId == order).OrderBy(row => row.StartTime).ToList();
            //if (data.Count == 0)
            //{
            //    return Success("false");
            //}
            data.Add("0", "休息");
            var treeList = new List<TreeEntity>();
            //TreeEntity treeBase = new TreeEntity();
            //treeBase.id = "";
            //treeBase.text = "休息";
            //treeBase.value = "";
            //treeBase.isexpand = true;
            //treeBase.complete = true;
            //treeBase.hasChildren = false;
            //treeBase.parentId = "0";
            //treeList.Add(treeBase);
            foreach (var item in data)
            {
                TreeEntity tree = new TreeEntity();
                if (item.Key=="0")
                {
                    tree.id ="1";
                }
                else
                {
                    tree.id = item.Key;
                }
                tree.id = item.Key;
                tree.text = item.Value;
                tree.value = item.Key;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 获取班次列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWorksetting(string worktype)
        {
            var user = OperatorProvider.Provider.Current();
            var data = workset.GetList(user.DeptId).Where(row => row.WorkSetupId == worktype).OrderBy(row => row.StartTime).ToList();
            if (data.Count == 0)
            {
                return Success("false");
            }
            var treeList = new List<TreeEntity>();
            TreeEntity treeBase = new TreeEntity();
            treeBase.id = "";
            treeBase.text = "休息";
            treeBase.value = "";
            treeBase.isexpand = true;
            treeBase.complete = true;
            treeBase.hasChildren = false;
            treeBase.parentId = "0";
            treeList.Add(treeBase);
            foreach (var item in data)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.WorkSettingId;
                tree.text = item.Name;
                tree.value = item.WorkSettingId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                treeList.Add(tree);
            }

            return Content(treeList.TreeToJson());
        }

        /// <summary>
        /// 班次列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetData()
        {
            var user = OperatorProvider.Provider.Current();
            var data = workset.GetList(user.DeptId).ToList();
            //分组后按时间排序
            //var dataList = from a in data
            //                group a by a.BookMarks into grp
            //                let ascTime = grp.OrderBy(row => row.StartTime)
            //                from row
            //                in grp
            //                select row;
            var dataList = from a in data
                           group a by a.BookMarks into grp
                           select new
                           {
                               BookMarks = grp.Key,
                               Accounts = from y in grp orderby y.StartTime ascending select y
                           };

            //获取创建班次的分组
            var dataGroup = from a in data
                            group a by new { a.BookMarks, a.WorkSetName }
                            into g
                            select new
                            {
                                worksetName = g.Key.WorkSetName,
                                workmarks = g.Key.BookMarks

                            };

            var treeList = new List<TreeGridEntity>();
            //循环出第一级
            foreach (var item in dataGroup)
            {
                TreeGridEntity tree = new TreeGridEntity();
                tree.id = item.workmarks;
                tree.hasChildren = true;
                tree.parentId = "0";
                tree.expanded = true;
                string itemJson = item.ToJson();
                itemJson = itemJson.Insert(1, "\"WorkId\":\"" + item.workmarks + "\",");
                itemJson = itemJson.Insert(1, "\"ParentId\":\"0\",");
                itemJson = itemJson.Insert(1, "\"Sort\":\"Work\",");
                itemJson = itemJson.Insert(1, "\"WorkName\":\"" + item.worksetName + "\",");
                tree.entityJson = itemJson;
                treeList.Add(tree);
            }
            foreach (var item in dataList)
            {
                foreach (var items in item.Accounts)
                {

                    TreeGridEntity tree = new TreeGridEntity();
                    tree.id = items.WorkSettingId;
                    tree.hasChildren = false;
                    tree.parentId = item.BookMarks;
                    tree.expanded = true;
                    string itemJson = items.ToJson();
                    itemJson = itemJson.Insert(1, "\"Sort\":\"WorkSetting\",");
                    itemJson = itemJson.Insert(1, "\"ParentId\":\"" + item.BookMarks + "\",");
                    itemJson = itemJson.Insert(1, "\"WorkTime\":\"" + items.StartTime.ToString("HH") + ":" + items.StartTime.ToString("mm") + "-" + items.EndTime.ToString("HH") + ":" + items.EndTime.ToString("mm") + "\",");
                    itemJson = itemJson.Insert(1, "\"WorkName\":\"" + items.Name + "\",");
                    itemJson = itemJson.Insert(1, "\"WorkNames\":\"" + items.Name + "\",");
                    itemJson = itemJson.Insert(1, "\"WorkTimes\":\"" + items.StartTime.ToString("HH") + ":" + items.StartTime.ToString("mm") + "-" + items.EndTime.ToString("HH") + ":" + items.EndTime.ToString("mm") + "\",");

                    tree.entityJson = itemJson;
                    treeList.Add(tree);
                }
            }
            return Content(treeList.TreeJson());
            //return View();
        }
        #endregion



        #region 提交数据
        /// <summary>
        /// 修改部门单天班次信息
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "修改部门单天班次信息")]
        public ActionResult WorkSetSaveOneDay(DateTime UpTime, string worksetting, string WorkOrderId)
        {
            try
            {
                workorder.WorkSetSaveOneDay(UpTime, worksetting, WorkOrderId);
                return Success("操作成功。");
            }
            catch (Exception)
            {

                return Error("操作失败。");
            }

        }
        /// <summary>
        /// 保存或修改部门班次信息
        /// </summary>

        /// <returns></returns>

        public ActionResult nextyear(DateTime startTime)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                var useEndTime = Convert.ToDateTime(startTime.ToString("yyyy-MM-01"));
                var dept = departmentBLL.GetAuthorizationDepartment(user.DeptId);
                var selectStart = DateTime.Now.Date.AddYears(-10);
                var selectEnd = DateTime.Now.Date.AddYears(10);
                if (useEndTime >= selectEnd)
                {
                    return Success("基础数据查询区间为十年，请重新操作。");
                }
                var order = workorder.GetWorkOrderList(selectStart, selectEnd, dept.DepartmentId);
                if (order.Count() == 0)
                {
                    return Error("该用户不具有基础数据操作查询权限或无排版基础数据。");
                }
                else
                {
                    var lastYear = order.OrderByDescending(x => x.year).FirstOrDefault().year;
                    lastYear++;
                    while (true)
                    {

                        if (lastYear > useEndTime.Year)
                        {
                            break;
                        }
                        var nextDate = new DateTime(lastYear, 1, 1);
                        workorder.nextyear(nextDate);
                        lastYear++;
                    }
                }
                return Success("操作成功。");
            }
            catch (Exception)
            {
                return Error("操作失败。");

            }

        }



        /// <summary>
        /// 保存或修改部门班次信息
        /// </summary>

        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存或修改排班信息")]
        public ActionResult WorkSetSave(string GroupJson, string setJson, string workSet, string workSetName)
        {
            try
            {

                var user = OperatorProvider.Provider.Current();
                var GroupJsonArry = GroupJson.ToList<WorkDeparmentList>();
                var setJsonnArry = setJson.ToList<WorksetList>();
                var data = new List<WorkOrderEntity>();
                var group = new List<WorkGroupSetEntity>();
                var bookmarks = Guid.NewGuid().ToString();
                if (workSetName == "常白班")
                {
                    int i = 0;
                    foreach (var item in GroupJsonArry)
                    {
                        bookmarks = Guid.NewGuid().ToString();
                        var setModel = new WorkOrderEntity();
                        setModel.WorkOrderId = Guid.NewGuid().ToString();
                        var setbase = workset.getEntitybySetUp(workSet, user.DeptId);
                        setModel.StartTime = setbase.StartTime;
                        setModel.EndTime = setbase.EndTime;
                        setModel.bookmarks = bookmarks;
                        setModel.isweek = workSetName == "常白班" ? true : false;
                        setModel.StartTimeSpan = setbase.StartTimeSpan;
                        setModel.EndTimeSpan = setbase.EndTimeSpan;
                        setModel.WorkName = workSetName;
                        setModel.OrderSort = 0;
                        setModel.CreateTime = DateTime.Now;
                        setModel.useTime = DateTime.Now;
                        setModel.CreateUserId = user.UserId;
                        setModel.settingid = setbase.WorkSettingId;
                        setModel.setupId = workSet;
                        data.Add(setModel);
                        var setGroupModel = new WorkGroupSetEntity();
                        setGroupModel.workgroupid = Guid.NewGuid().ToString();
                        setGroupModel.departmentid = item.DepartmentId;
                        setGroupModel.fullname = item.FullName;
                        //var setGroupbase = data.FirstOrDefault(x => x.OrderSort == i);
                        setGroupModel.groupsort = data[i].OrderSort;
                        setGroupModel.workorderid = data[i].WorkOrderId;
                        setGroupModel.CreateTime = DateTime.Now;
                        setGroupModel.createuserid = user.UserId;
                        setGroupModel.bookmarks = bookmarks;
                        group.Add(setGroupModel);
                        i++;
                    }

                }
                else
                {
                    foreach (var item in setJsonnArry)
                    {
                        var setModel = new WorkOrderEntity();
                        setModel.WorkOrderId = Guid.NewGuid().ToString();
                        if (item.text != "休息")
                        {
                            var setbase = workset.getEntity(item.value);
                            setModel.StartTime = setbase.StartTime;
                            setModel.EndTime = setbase.EndTime;
                            setModel.isweek = workSetName == "常白班" ? true : false;
                            setModel.StartTimeSpan = setbase.StartTimeSpan;
                            setModel.EndTimeSpan = setbase.EndTimeSpan;
                            setModel.settingid = setbase.WorkSettingId;
                        }
                        else
                        {
                            setModel.StartTime = DateTime.Now;
                            setModel.EndTime = DateTime.Now;
                            setModel.isweek = workSetName == "常白班" ? true : false;
                            setModel.StartTimeSpan = 0;
                            setModel.EndTimeSpan = 0;
                            setModel.settingid = "0";
                        }
                        setModel.bookmarks = bookmarks;
                        setModel.WorkName = item.text;
                        setModel.OrderSort = item.sort;
                        setModel.CreateTime = DateTime.Now;
                        setModel.CreateUserId = user.UserId;
                        setModel.useTime = DateTime.Now;
                        setModel.setupId = workSet;
                        data.Add(setModel);
                    }
                    foreach (var item in GroupJsonArry)
                    {
                        var setModel = new WorkGroupSetEntity();
                        setModel.workgroupid = Guid.NewGuid().ToString();
                        setModel.departmentid = item.DepartmentId;
                        setModel.fullname = item.FullName;
                        var setbase = data.FirstOrDefault(x => x.OrderSort == item.selectValue);
                        setModel.groupsort = setbase.OrderSort;
                        setModel.workorderid = setbase.WorkOrderId;
                        setModel.CreateTime = DateTime.Now;
                        setModel.createuserid = user.UserId;
                        setModel.bookmarks = bookmarks;
                        group.Add(setModel);
                    }
                }

                workorder.WorkSetSave(data, group, DateTime.Now);
                return Success("操作成功。");
            }
            catch (Exception)
            {
                return Error("操作失败。");

            }

        }

        /// <summary>
        /// 修改班制启用状态
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "修改班制启用状态")]
        public ActionResult EditState(string WorkId)
        {

            //var worksettingList = workset.GetList().Where(row => row.BookMarks == WorkId).ToList();
            //var workSet = workset.GetList().Where(row => row.BookMarks != WorkId && row.WorkSetupId == worksettingList[0].WorkSetupId).ToList();
            //if (worksettingList.Count > 0)
            //{
            //    foreach (var item in workSet)
            //    {
            //        item.WorkState = false;
            //    }
            //    workset.saveForm(workSet);
            //}
            //foreach (var item in worksettingList)
            //{
            //    item.WorkState = true;

            //}
            //workset.saveForm(worksettingList);
            return Success("操作成功。");
        }

        /// <summary>
        /// 保存班次（新增、修改）
        /// </summary>
        /// <param name="WorksetType">班制类型</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)班次信息")]
        public ActionResult SaveForm(string WorksetType, WorkSettingEntity entity)
        {
            var user = OperatorProvider.Provider.Current();
            List<WorkSettingEntity> entityList = new List<WorkSettingEntity>();
            if (WorksetType != "update")
            {
                //获取班制类别
                var data = sysBll.GetList().FirstOrDefault(row => row.Id == WorksetType);
                //是否已经存在
                var one = workset.GetList(user.DeptId).FirstOrDefault(row => row.WorkSetName == data.SystemName);
                if (one != null)
                {
                    return Success("已经存在该班次。");
                }
                else
                {
                    //初始化班制
                    entityList = SaveDate(data);
                }
            }
            else
            {
                var one = workset.GetList(user.DeptId).FirstOrDefault(row => row.WorkSettingId == entity.WorkSettingId);
                one.TimeLength = entity.TimeLength;
                one.Name = entity.Name;
                one.StartTimeSpan = entity.StartTimeSpan;
                one.EndTimeSpan = entity.EndTimeSpan;
                DateTime markTime = new DateTime(2000, 01, 02);
                if (entity.EndTime <= entity.StartTime)
                {
                    //跨天
                    entity.EndTime = markTime.AddDays(1).AddHours(entity.EndTime.Hour).AddMinutes(entity.EndTime.Minute);
                    entity.StartTime = markTime.AddHours(entity.StartTime.Hour).AddMinutes(entity.StartTime.Minute);
                }
                else
                {
                    //没有跨天
                    entity.EndTime = markTime.AddHours(entity.EndTime.Hour).AddMinutes(entity.EndTime.Minute);
                    entity.StartTime = markTime.AddHours(entity.StartTime.Hour).AddMinutes(entity.StartTime.Minute);
                }
                one.EndTime = entity.EndTime;
                one.StartTime = entity.StartTime;
                entityList.Add(one);
            }
            workset.saveForm(entityList);
            return Success("操作成功。");
        }


        /// <summary>
        /// 计算班制初始数据
        /// </summary>
        /// <returns></returns>
        private List<WorkSettingEntity> SaveDate(WorkSetTypeEntity data)
        {
            List<WorkSettingEntity> entityList = new List<WorkSettingEntity>();
            int typeNum = Convert.ToInt32(data.SystemType);
            DateTime markTime = new DateTime(2000, 01, 02);
            string randomNum = CommonHelper.RndNum(8);
            var user = OperatorProvider.Provider.Current();
            for (int i = 0; i < typeNum; i++)
            {
                int hourNum = 24 / typeNum;
                WorkSettingEntity entity = new WorkSettingEntity();
                entity.DeparMentId = user.DeptId;
                entity.BookMarks = randomNum;
                entity.StartTime = data.SystemName == "常白班" ? markTime.AddHours(8).AddMinutes(30) : data.SystemName == "一班制" ? markTime : markTime.AddHours(hourNum * i);
                if (i == typeNum - 1)
                {
                    entity.EndTime = data.SystemName == "常白班" ? markTime.AddHours(17).AddMinutes(30) : data.SystemName == "一班制" ? markTime.AddDays(1).AddMinutes(-1) : markTime.AddHours(hourNum * (i + 1)).AddMinutes(-1);

                }
                else
                {
                    entity.EndTime = data.SystemName == "常白班" ? markTime.AddHours(17).AddMinutes(30) : data.SystemName == "一班制" ? markTime.AddDays(1).AddMinutes(-1) : markTime.AddHours(hourNum * (i + 1));

                }
                var timeSpan = (entity.EndTime - entity.StartTime);
                entity.TimeLength = timeSpan.Hours < 10 ? "0" + timeSpan.Hours + "时" + (timeSpan.Minutes < 10 ? "0" + timeSpan.Minutes.ToString() + "分" : timeSpan.Minutes.ToString() + "分")
                    : timeSpan.Hours.ToString() + "时" + (timeSpan.Minutes < 10 ? "0" + timeSpan.Minutes.ToString() + "分" : timeSpan.Minutes.ToString() + "分");
                entity.WorkSetType = typeNum;
                entity.WorkSetupId = data.Id;
                entity.WorkSetName = data.SystemName;
                #region  班次名称
                switch (data.SystemName)
                {
                    case "常白班":
                        entity.Name = "常白班";
                        break;
                    case "一班制":
                        entity.Name = "通宵班";
                        break;
                    case "两班制":
                        switch (i)
                        {
                            case 0:
                                entity.Name = "白班";
                                break;
                            default:
                                entity.Name = "夜班";
                                break;
                        }
                        break;
                    case "三班制":
                        switch (i)
                        {
                            case 0:
                                entity.Name = "前夜";
                                break;
                            case 1:
                                entity.Name = "白班";
                                break;
                            default:
                                entity.Name = "后夜";
                                break;
                        }
                        break;
                    case "四班制":
                        switch (i)
                        {
                            case 0:
                                entity.Name = "A班";
                                break;
                            case 1:
                                entity.Name = "B班";
                                break;
                            case 2:
                                entity.Name = "C班";
                                break;
                            default:
                                entity.Name = "D班";
                                break;
                        }
                        break;
                    default:
                        entity.Name = "班制";
                        break;
                }
                #endregion  
                entityList.Add(entity);
            }
            return entityList;
        }

        /// <summary>
        /// 删除班制一批量班次
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除班次信息")]
        public ActionResult RemoveForm(string keyValue)
        {

            if (workset.RemoveForm(keyValue))
            {
                return Success("删除成功。");  //删除成功
            }
            else
            {
                return Success("删除失败。");
            }
        }
        /// <summary>
        /// 批量删除排班
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        //[HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除排班信息")]
        public ActionResult OrderRemoveForm(string keyValue)
        {

            if (workorder.OrderRemoveForm(keyValue))
            {
                return Success("删除成功。");  //删除成功
            }
            else
            {
                return Success("删除失败。");
            }
        }

        #endregion
    }
}