using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.PublicInfoManage.ViewMode;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Application.Web.Areas.SystemManage.Models;
using BSFramework.Cache.Factory;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 首页指标配置
    /// </summary>
    public class IndexManageController : MvcControllerBase
    {
        private IndexManageBLL manageBLL = new IndexManageBLL();
        private DataSetBLL scoresetbll = new DataSetBLL();

        #region 视图
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TitleIndex()
        {
            return View();
        }

        public ActionResult TitleForm()
        {
            return View();
        }
        public ActionResult BindForm(string keyValue, int indexType)
        {
            if (indexType != 0)//为1则是安卓终端的指标配置
            {
                return RedirectToAction("TerminalBindForm", new { keyValue, indexType });
            }
            Operator user = OperatorProvider.Provider.Current();
            DataSetBLL dataSetBLL = new DataSetBLL();
            var indexlist = dataSetBLL.GetList(null).Where(p => p.IsOpen == 1).ToList(); //所有的指标
            //查找所有的关联关系
            IIndexAssocationBLL indexAssocationBLL = new IIndexAssocationBLL();
            List<IndexAssocationEntity> indices = indexAssocationBLL.GetList(user.DeptId);
            //查询未被其他标题绑定的指标
            var BindIds = indices.Where(x => x.TitleId != keyValue).Select(p => p.DataSetId);//非当前标题 已经的被帮的指标
            var thisBindIds = indices.Where(x => x.TitleId == keyValue).Select(p => p.DataSetId);//当前标题绑定的指标
            var data = indexlist.Where(x => !BindIds.Contains(x.Id)).ToList();//当前标题 可显示的指标
            ViewBag.ChceckIds = thisBindIds.ToList();
            ViewBag.TitleId = keyValue;
            return View(data);
        }

        /// <summary>
        /// 给栏目绑定指标 页面
        /// </summary>
        /// <param name="keyValue">栏目Id</param>
        /// <param name="indexType">所属平台 0web平台 1安卓平台 2App</param>
        /// <param name="Templet">所属栏目</param>
        /// <returns></returns>
        public ActionResult TerminalBindForm(string keyValue, int indexType, int Templet)
        {
            Operator user = OperatorProvider.Provider.Current();
            DepartmentEntity userdept = new DepartmentBLL().GetCompany(user.DeptId);
            TerminalDataSetBLL dataSetBLL = new TerminalDataSetBLL();
            List<TerminalDataSetEntity> indexlist = new List<TerminalDataSetEntity>();
            indexlist = dataSetBLL.GetList().Where(p => p.IsOpen == 1).ToList();
            //if (indexType == 2)
            //{
            //    indexlist = dataSetBLL.GetList().Where(p => p.IsOpen == 1 && (p.DataSetType == indexType.ToString() )).ToList(); //所有的手机APP指标
            //}
            //else
            //{
            //    indexlist = dataSetBLL.GetList().Where(p => p.IsOpen == 1 && (p.DataSetType == indexType.ToString() || p.DataSetType == null)).ToList(); //所有的终端指标
            //}

            //查询本IndexType的栏目
            var titleIds = manageBLL.GetList(userdept.DepartmentId, indexType).Where(x => x.Templet == Templet).Select(x => x.Id);
            //查找所有的关联关系
            IIndexAssocationBLL indexAssocationBLL = new IIndexAssocationBLL();
            List<IndexAssocationEntity> indices = indexAssocationBLL.GetListByTitleId(titleIds.ToArray());
            //先剔除非本IndexType的关联关系
            indices = indices.Where(x => titleIds.Contains(x.TitleId)).ToList();
            //查询未被其他标题绑定的指标
            var BindIds = indices.Where(x => x.TitleId != keyValue).Select(p => p.DataSetId);//非当前标题 已经的被帮的指标
            var thisBindIds = indices.Where(x => x.TitleId == keyValue).Select(p => p.DataSetId);//当前标题绑定的指标
            var data = indexlist.Where(x => !BindIds.Contains(x.Id)).ToList();//当前标题 可显示的指标
            ViewBag.ChceckIds = thisBindIds.ToList();
            ViewBag.TitleId = keyValue;
            return View(data);
        }

        public ActionResult TerminalDataSetIndex()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult AppDataSet()
        {
            return View();
        }
        public ActionResult TerminalForm()
        {
            return View();
        }
        public ActionResult AppDataSetForm()
        {
            return View();
        }


        #endregion
        #region 方法
        /// <summary>
        /// 获取指标分类的栏目
        /// </summary>
        /// <param name="keyword">查询关键字</param>
        /// <param name="indexType">指标类型  0管理平台  1安卓终端 2手机APP</param>
        /// <param name="templet">所属模板</param>
        /// <returns></returns>
        public ActionResult GetTitleList(string keyword, int indexType, int templet)
        {
            var companyDept = new DepartmentBLL().GetCompany(OperatorProvider.Provider.Current().DeptId);
            List<IndexManageEntity> entities = manageBLL.GetList(companyDept.DepartmentId, indexType, keyword, templet);
            //List<IndexModel> models = new List<IndexModel>();
            //entities.ForEach(x =>
            //{
            //    models.Add(new IndexModel(x));
            //});
            return Json(entities, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveTitle(string keyValue, IndexManageEntity entity)
        {
            try
            {
                //找当前登录人所在的组织的ID  厂级或厂级以下找 厂级ID  省级找省级ID    集团级找集团ID
                Operator user = OperatorProvider.Provider.Current();
                DepartmentEntity orgDept = new DepartmentBLL().GetCompany(user.DeptId);
                entity.DeptId = orgDept.DepartmentId;
                entity.DeptCode = orgDept.EnCode;
                entity.DeptName = orgDept.FullName;
                manageBLL.SaveForm(keyValue, entity);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public ActionResult GetTitleFormJson(string keyValue)
        {
            IndexManageEntity indexManage = manageBLL.GetForm(keyValue);
            return Json(indexManage, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveTitle(string keyValue)
        {
            try
            {
                manageBLL.Remove(keyValue);
                //删除与指标的关联关系
                IIndexAssocationBLL assocationBLL = new IIndexAssocationBLL();
                //先查出该标题下所有的指标
                List<IndexAssocationEntity> entities = assocationBLL.GetListByTitleId(keyValue);
                assocationBLL.Remove(entities);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }
        #endregion

        #region 视图功能

        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }

        /// <summary>
        /// web平台指标表单页 
        /// </summary>
        public ActionResult WebDataSetFrom()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取平台指标列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //pagination.p_kid = "Id";
            //pagination.p_fields = "itemcode,itemname,itemtype,itemrole,isdefault,isopen,deptcode,deptname,CreateDate,itemkind,sortcode,address";
            //pagination.p_tablename = "BASE_Dataset";
            //pagination.conditionJson = " 1=1 ";
            //pagination.sidx = "sortcode";
            //pagination.sord = "asc";
            Operator user = OperatorProvider.Provider.Current();
            var data = scoresetbll.GetPageList(pagination, queryJson);
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
        /// 获取安卓终端指标列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <param name="DataSetType">0或者null 是 安卓终端  1是手机app端</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTerminalPageListJson(Pagination pagination, string queryJson, string DataSetType = null)
        {
            var watch = CommonHelper.TimerStart();
            TerminalDataSetBLL terminalDataSetBLL = new TerminalDataSetBLL();
            //pagination.p_kid = "id";
            //pagination.p_fields = "name,sort,remark,isopen,code,unint,createdate,BK1,BK2,BK3,BK4";
            //pagination.p_tablename = "Base_TerminalDataSet";
            //pagination.conditionJson = " 1=1 ";
            //pagination.sidx = "sort";
            //pagination.sord = "asc";
            Operator user = OperatorProvider.Provider.Current();
            var data = terminalDataSetBLL.GetPageList(pagination, queryJson, DataSetType);
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
        /// 获取实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var entity = scoresetbll.GetEntity(keyValue);
            return ToJsonResult(entity);
        }

        /// <summary>
        /// 从双控获模块菜单
        /// </summary>
        /// <param name="platform">平台类型 0 windows 1安卓终端 2手机APP</param>
        /// <returns></returns>
        public ActionResult GetMenuList(int platform)
        {
            try
            {
                var columnMenu = new TerminalDataSetBLL().GetMenuConfigList(platform);
                var firstLevelMenus = columnMenu.Where(x => x.ParentId == platform.ToString());
                List<MenuTreeModel> data = new List<MenuTreeModel>();
                foreach (var firstmenu in firstLevelMenus)
                {
                    data.Add(new MenuTreeModel(firstmenu, platform.ToString()));
                }
                foreach (var firstLevel in data)
                {
                    MenuTreeHelper.FomateTree(firstLevel, columnMenu);
                }
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new List<MenuTreeModel>(), JsonRequestBehavior.AllowGet);
            }

        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string keyValue)
        {
            scoresetbll.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        ///  <param name="score">初始积分值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, DataSetEntity ds)
        {
            if (ds.ItemStyle.Trim().Length > 0)
            {
                ds.ItemStyle = HttpUtility.UrlDecode(ds.ItemStyle);
            }
            scoresetbll.SaveForm(keyValue, ds);
            return Success("操作成功。");
        }
        /// <summary>
        /// 保存指标与标题的关联关系
        /// </summary>
        /// <param name="titleId">标题ID</param>
        /// <param name="indexIdStr">指标的ID的集合</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveAssociation(string titleId, string indexIdStr)
        {
            try
            {
                List<string> indexIds = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(indexIdStr);
                IIndexAssocationBLL assocationBLL = new IIndexAssocationBLL();
                //先查出该标题下所有的指标
                List<IndexAssocationEntity> oldEntity = assocationBLL.GetListByTitleId(titleId);
                //要删除的数据
                List<IndexAssocationEntity> delEntity = oldEntity.Where(x => !indexIds.Contains(x.DataSetId)).ToList();
                //要新增进去的数据
                var oldids = oldEntity.Select(o => o.DataSetId).ToList();
                var addIds = indexIds.Where(p => !oldids.Contains(p)).ToList();
                Operator user = OperatorProvider.Provider.Current();
                List<IndexAssocationEntity> addEntitys = new List<IndexAssocationEntity>();

                addIds.ForEach(setId =>
                {
                    addEntitys.Add(new IndexAssocationEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        DataSetId = setId,
                        DeptId = user.DeptId,
                        TitleId = titleId
                    });
                });
                assocationBLL.Remove(delEntity);
                assocationBLL.Insert(addEntitys);

                return Success("操作成功");
            }
            catch (Exception ex)
            {
                return Error("保存失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 保存终端指标数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="ds"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveTerminalForm(string keyValue, TerminalDataSetEntity ds)
        {
            try
            {
                TerminalDataSetBLL bll = new TerminalDataSetBLL();
                bll.SaveForm(keyValue, ds);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

        }

        /// <summary>
        /// 获取终端指标配置实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetTerminalFormJson(string keyValue)
        {
            TerminalDataSetBLL bll = new TerminalDataSetBLL();
            var entity = bll.GetEntity(keyValue);
            return ToJsonResult(entity);
        }
        /// <summary>
        /// 删除终端指标
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveTerminalForm(string keyValue)
        {
            try
            {
                TerminalDataSetBLL bll = new TerminalDataSetBLL();
                bll.RemoveForm(keyValue);
                return Success("删除成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
        }

        public ActionResult SaveIcon()
        {
            HttpFileCollection files = System.Web.HttpContext.Current.Request.Files;
            //没有文件上传，直接返回
            if (files[0].ContentLength == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return HttpNotFound();
            }
            string virtualPath = "";
            string UserId = OperatorProvider.Provider.Current().UserId;
            string FileEextension = Path.GetExtension(files[0].FileName);

            virtualPath = string.Format("/Resource/MenuIconFile/{0}{1}", Guid.NewGuid().ToString(), FileEextension);
            string fullFileName = Server.MapPath("~" + virtualPath);
            //创建文件夹，保存文件
            string path = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(path);
            files[0].SaveAs(fullFileName);

            return Success("上传成功。", virtualPath);
        }
        #endregion


        #region 管理平台首页
        #region 班组建设指标
        /// <summary>
        /// 首页指标Ifreame 页面
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexPartView()
        {

            return View();
        }

        public JsonResult GetIndexManageJson()
        {
            Operator user = OperatorProvider.Provider.Current();
            DepartmentBLL departmentBLL = new DepartmentBLL();
            AdminPrettyBLL adminPrettyBLL = new AdminPrettyBLL();
            //先查询出所有的分类
            DepartmentEntity userdept = departmentBLL.GetCompany(user.DeptId);
            List<IndexManageModel> indexDatas = new List<IndexManageModel>();
            indexDatas = CacheFactory.Cache().GetCache<List<IndexManageModel>>("bz_index_indexManage");
            if (indexDatas == null)
            {
                indexDatas = adminPrettyBLL.GetIndexData(userdept.DepartmentId, user.UserId, 0, user.DeptId, null, 1);
                CacheFactory.Cache().WriteCache(indexDatas, "bz_index_indexManage", DateTime.Now.AddHours(5));
            }
            return Json(indexDatas, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 实时预警
        /// <summary>
        /// 实时预警
        /// </summary>
        /// <returns></returns>
        public ActionResult ActualWarning()
        {
            return View();
        }
        /// <summary>
        /// 实时
        /// </summary>
        public class IndexActualModel
        {
            /// <summary>
            /// 名称
            /// </summary>
            public string name { get; set; }
            /// <summary>   
            /// 数量
            /// </summary>
            public int num { get; set; }
            /// <summary>
            /// 排序
            /// </summary>
            public int sort { get; set; }


        }
        /// <summary>
        /// 根据配置获取页面展示
        /// </summary>
        public JsonResult GetActualWarning()
        {
            var user = OperatorProvider.Provider.Current();
            var traingtype = Config.GetValue("TrainingType");
            var CustomerModel = Config.GetValue("CustomerModel");
            var data = new List<IndexActualModel>();

            data = CacheFactory.Cache().GetCache<List<IndexActualModel>>("ActualWarning_" + user.DeptId);
            if (data != null)
            {
                return Json(data);
            }
            else
            {
                data = new List<IndexActualModel>();
            }
            //获取双控菜单配置
            var MenuData = new TerminalDataSetBLL().GetMenuConfigList(1);
            var RealTimeBool = new bool[] { false, false, false, true, false };
            var one = MenuData.Where(x => x.ModuleName == "班前班后会").FirstOrDefault();//班前班后会
            if (one != null)
            {
                RealTimeBool[0] = true;
            }
            var two = MenuData.Where(x => x.ModuleName == "班组活动").FirstOrDefault();//班组活动
            if (two != null)
            {
                RealTimeBool[1] = true;
            }
            var three = MenuData.Where(x => x.ModuleName == traingtype || x.ModuleName == CustomerModel).FirstOrDefault();//人身风险预控 or CustomerModel
            var ModuleName = string.Empty;
            if (three != null)
            {
                RealTimeBool[2] = true;
                ModuleName = three.ModuleName;
            }
            var five = MenuData.Where(x => x.ModuleName == "教育培训").FirstOrDefault();//教育培训
            if (five != null)
            {
                RealTimeBool[4] = true;
            }

            var DictionaryData = new IndexManageBLL().RealTime(user.DeptId, ModuleName, RealTimeBool, user.UserId);



            //排序 
            foreach (var item in DictionaryData)
            {
                var One = new IndexActualModel()
                {
                    name = item.Key,
                    num = item.Value,
                    sort = item.Key.Contains("连续一周") ? 1 : item.Key.Contains("连续两周") ? 2 : item.Key.Contains("今日未开展人身风险预控") ? 3 : item.Key.Contains("14天未开展") ? 4 : item.Key.Contains("未整改违章") ? 5 : item.Key.Contains("未整改隐患") ? 6 : 7,

                };
                data.Add(One);
            }
            data = data.OrderBy(x => x.sort).ToList();
            CacheFactory.Cache().WriteCache(data, "ActualWarning_" + user.DeptId, DateTime.Now.AddHours(5));
            return Json(data);
        }


        #endregion
        #region 今日班前班后会
        /// <summary>
        /// 今日班前班后会
        /// </summary>
        /// <returns></returns>
        public ActionResult TodayWorkmeet()
        {
            return View();
        }

        /// <summary>
        ///获取今日班会信息
        /// </summary>
        public JsonResult GetTodayWorkmeet()
        {
            var user = OperatorProvider.Provider.Current();
            var data = new IndexManageBLL().GetDeptsMeet(user.DeptId);
            return Json(data);
        }

        #endregion

        #region 今日工作任务
        /// <summary>
        /// 今日工作任务
        /// </summary>
        /// <returns></returns>
        public ActionResult TodayJob()
        {
            return View();
        }

        /// <summary>
        ///获取今日统计数据
        /// </summary>
        public JsonResult GetTodayJob()
        {
            var user = OperatorProvider.Provider.Current();
            var data = new IndexManageBLL().TodayWorkStatistics(user.DeptId);
            return Json(data);
        }

        #endregion


        #region 教育培训统计

        /// <summary>
        /// 教育培训统计
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexEduManage()
        {
            return View();
        }
        /// <summary>
        ///教育培训统计
        /// </summary>
        public JsonResult GetIndexEduManage(string type)
        {
            var user = OperatorProvider.Provider.Current();
            DateTime now = DateTime.Now.Date;
            DateTime start = now, end = now;

            switch (type)
            {
                case "1":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddMilliseconds(-1);
                    break;
                case "2":
                    var monthNum = (now.Month + 2) / 3;
                    start = new DateTime(now.Year, (monthNum - 1) * 3 + 1, 1);
                    end = start.AddMonths(3).AddMilliseconds(-1);
                    break;
                case "3":
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1).AddMilliseconds(-1);
                    break;
                default:
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddMilliseconds(-1);
                    break;
            }
            var data = CacheFactory.Cache().GetCache<Dictionary<string, int>>("EduManage_" + type + "_" + user.DeptId);
            if (data != null)
            {
                return Json(data);
            }

            data = new IndexManageBLL().GetEdCount(user.DeptId, start, end);
            CacheFactory.Cache().WriteCache<Dictionary<string, int>>(data, "EduManage_" + type + "_" + user.DeptId, DateTime.Now.AddHours(5));
            return Json(data);
        }


        #endregion

        #region 隐患趋势
        /// <summary>
        /// 隐患趋势
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexHiddenDanger()
        {
            return View();
        }


        /// <summary>
        ///获取隐患趋势
        /// </summary>
        public JsonResult GetIndexHiddenDanger(string year)
        {
            var user = OperatorProvider.Provider.Current();

            var data = CacheFactory.Cache().GetCache<Dictionary<string, Dictionary<int, int>>>("HiddenDanger_" + year + "_" + user.UserId);
            if (data != null)
            {
                var resultData = data.Select(x => new { name = x.Key, month = x.Value.Select(p => p.Key).ToList(), monthdata = x.Value.Select(p => p.Value).ToList() }).ToList();

                return Json(resultData);
            }
      
            data = new IndexManageBLL().HiddenDanger(user.UserId, year);
            CacheFactory.Cache().WriteCache<Dictionary<string, Dictionary<int, int>>>(data, "HiddenDanger_" + year + "_" + user.UserId, DateTime.Now.AddHours(5));
            var result = data.Select(x => new { name = x.Key, month = x.Value.Select(p => p.Key).ToList(), monthdata = x.Value.Select(p => p.Value).ToList() }).ToList();

            return Json(result);
        }


        #endregion

        #region 人身风险预控

        /// <summary>
        /// 人身风险预控
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexHDCount()
        {
            return View();
        }
        /// <summary>
        ///教育培训统计
        /// </summary>
        public JsonResult GetIndexHDCount(string type)
        {
            var user = OperatorProvider.Provider.Current();
            DateTime now = DateTime.Now.Date;
            DateTime start = now, end = now;
            switch (type)
            {
                case "1":
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddMilliseconds(-1);
                    break;
                case "2":
                    var monthNum = (now.Month + 2) / 3;
                    start = new DateTime(now.Year, (monthNum - 1) * 3 + 1, 1);
                    end = start.AddMonths(3).AddMilliseconds(-1);
                    break;
                case "3":
                    start = new DateTime(now.Year, 1, 1);
                    end = start.AddYears(1).AddMilliseconds(-1);
                    break;
                default:
                    start = new DateTime(now.Year, now.Month, 1);
                    end = start.AddMonths(1).AddMilliseconds(-1);
                    break;
            }
            var data = CacheFactory.Cache().GetCache<Dictionary<string, int>>("HDCount_" + type + "_" + user.DeptId);
            if (data != null)
            {
                return Json(data);
            }
           
            var deptBll = new DepartmentBLL();
            //获取根节点
            var dept = deptBll.GetAuthorizationDepartment(user.DeptId);
            //获取所有班组信息 
            var depts = deptBll.GetSubDepartments(dept.DepartmentId, "");
            data = new IndexManageBLL().GetHDCount(user.UserId, depts, start, end);
            CacheFactory.Cache().WriteCache<Dictionary<string, int>>(data, "HDCount_" + type + "_" + user.DeptId, DateTime.Now.AddHours(5));
            return Json(data);
        }
        #endregion

        #endregion
    }
}
