using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Application.Entity.DeviceInspection;
using BSFramework.Application.Busines.DeviceInspection;
using Newtonsoft.Json;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using BSFramework.Util.Log;
using BSFramework.Application.Entity.PublicInfoManage;
using Aspose.Cells;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Application.Entity.BaseManage;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 巡检表管理 hm
    /// </summary>
    public class DeviceController : MvcControllerBase
    {
        /// <summary>
        /// 方法层 DeviceInspectionBLL
        /// </summary>
        private readonly DeviceInspectionBLL _bll;
        private readonly InspectionRecordBLL _recordBll;

        public DeviceController()
        {
            _bll = new DeviceInspectionBLL();
            _recordBll = new InspectionRecordBLL();
        }
        #region 巡检表管理

        #region 页面
        /// <summary>
        /// 巡检表管理
        /// </summary>
        /// <returns></returns>
        public ActionResult InspectionIndex()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            bool isSpecialUser = false;//是否是特殊部门
            if (user.IsSystem || dept.IsSpecial)
            {
                isSpecialUser = true;
            }
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.UserDeptCode = dept.EnCode;
            ViewBag.isSpecialUser = isSpecialUser.ToString();
            return View();
        }
        /// <summary>
        /// 巡检表详情页
        /// <param name="keyValue">主键</param>
        /// <param name="action">add 新增;edit修改 </param>
        /// </summary>
        /// <returns></returns>
        public ActionResult InspectionAdd(string keyValue)
        {
            DeviceInspectionEntity entity = _bll.GetEntity(keyValue);
            if (entity == null)
            {
                entity = new DeviceInspectionEntity();
                entity.Create();
            }
            List<DeviceInspectionItemEntity> items = _bll.GetDeviceInspectionItems(keyValue);
            ViewBag.Items = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(items));
            return View(entity);
        }

        /// <summary>
        /// 设备巡检表查看页面
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public ActionResult InspectionShow(string keyValue)
        {
            DeviceInspectionEntity entity = _bll.GetEntity(keyValue);
            List<DeviceInspectionItemEntity> items = _bll.GetDeviceInspectionItems(keyValue);
            ViewBag.Items = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(items));
            return View(entity);
        }

        /// <summary>
        /// excel导入页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportPage()
        {
            return View();
        }
        #endregion

        #region  方法
        /// <summary>
        /// 保存设备巡回检查信息
        /// </summary>
        /// <param name="keyValue">设备巡回检查主键</param>
        /// <param name="entity">设备巡回检查实体</param>
        /// <param name="itemsJson">检查项Json格式数据</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "保存/新增设备巡回检查表")]
        public ActionResult SaveForm(string keyValue, DeviceInspectionEntity entity, string itemsJson)
        {
            try
            {
                Operator user = OperatorProvider.Provider.Current();
                List<DeviceInspectionItemEntity> items = new List<DeviceInspectionItemEntity>();//检查项
                if (!string.IsNullOrWhiteSpace(itemsJson))
                {
                    items = JsonConvert.DeserializeObject<List<DeviceInspectionItemEntity>>(itemsJson);
                }
                _bll.SaveForm(keyValue, entity, items);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"参数：body_{JsonConvert.SerializeObject(Request.Form)} QueryString_{Request.QueryString}\r\n错误消息：{JsonConvert.SerializeObject(ex)}", "DeviceInspection");
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 设备巡回检查表 检查表名称唯一性检查
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="InspectionName">检查表名称的值</param>
        /// <returns></returns>
        public ActionResult ExistInspectionName(string keyValue, string InspectionName)
        {
            try
            {
                bool isOk = _bll.ExistInspectionName(keyValue, InspectionName);
                return Content(isOk.ToString());
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        /// <summary>
        /// 分页查询设备巡回检查表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetDeviceInspectionJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            Operator user = OperatorProvider.Provider.Current();
            IEnumerable<DeviceInspectionEntity> data = _bll.GetPageList(pagination, queryJson).OrderByDescending(x => x.CreateDate);
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
        /// 删除分类
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "删除设备巡回检查表")]
        public ActionResult Remove(string keyValue)
        {
            try
            {
                _bll.RemoveInspection(keyValue);
                return Success("删除成功。");
            }
            catch (Exception ex)
            {
                return Error("删除失败：" + ex.Message);
            }

        }
        #endregion
        #endregion

        #region  巡回检查记录
        #region 页面 
        /// <summary>
        /// 台账页
        /// </summary>
        /// <returns></returns>
        public ActionResult RecordIndex()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            return View();
        }

        /// <summary>
        /// 检查记录详情
        /// </summary>
        /// <param name="keyValue">检查记录详情表主键</param>
        /// <returns></returns>
        public ActionResult RecordShow(string keyValue)
        {
            //1.获取单个检查记录数据
            InspectionRecordEntity entity = _recordBll.GetEntity(keyValue);
            //2.获取检查记录各项的信息
            var items = _recordBll.GetRecordItems(keyValue, entity.DeviceId);
            //3.附件
            IList<FileInfoEntity> files = new Busines.PublicInfoManage.FileInfoBLL().GetFilesByRecIdNew(entity.Id);
            ViewBag.Items = HttpUtility.JavaScriptStringEncode(JsonConvert.SerializeObject(items)); ;
            ViewBag.Files = files;
            return View(entity);
        }
        #endregion
        #region 方法 
        /// <summary>
        /// 分页查询设备巡回检查表
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetRecordPagedJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            Operator user = OperatorProvider.Provider.Current();
            IEnumerable<InspectionRecordEntity> data = _recordBll.GetPageList(pagination, queryJson).OrderByDescending(x => x.CreateDate);
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

        [HttpPost]
        [AjaxOnly]
        [HandlerMonitor(6, "导入设备巡回检查表")]
        public ActionResult Import()
        {
            Operator user = OperatorProvider.Provider.Current();
            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) return Json(new { success = false, message = "请上传excel文件！" });

                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];

                if (sheet.Cells[1, 0].StringValue != "检查表名称*" || sheet.Cells[1, 1].StringValue != "设备系统*" || sheet.Cells[1, 2].StringValue != "检查项目*" || sheet.Cells[1, 3].StringValue != "方法*" || sheet.Cells[1, 4].StringValue != "标准*" || sheet.Cells[1, 5].StringValue != "班组*")
                {
                    return Json(new { success = false, message = "请使用正确的模板导入！" });
                }

                bool success = true;
                string message = "导入成功";
                List<ImportInspectionModel> importList = new List<ImportInspectionModel>();
                for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                {
                    if (string.IsNullOrEmpty(sheet.Cells[i, 0].StringValue)) { success = false; message = "检查表名称不能为空"; break; }
                    if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue)) { success = false; message = "设备系统不能为空"; break; }
                    if (string.IsNullOrEmpty(sheet.Cells[i, 2].StringValue)) { success = false; message = "检查项目不能为空"; break; }
                    if (string.IsNullOrEmpty(sheet.Cells[i, 3].StringValue)) { success = false; message = "方法不能为空"; break; }
                    if (string.IsNullOrEmpty(sheet.Cells[i, 4].StringValue)) { success = false; message = "标准不能为空"; break; }
                    if (string.IsNullOrEmpty(sheet.Cells[i, 5].StringValue)) { success = false; message = "班组不能为空"; break; }

                    ImportInspectionModel import = new ImportInspectionModel()
                    {
                        InspectionName = sheet.Cells[i, 0].StringValue,
                        DeviceSystem = sheet.Cells[i, 0].StringValue,
                        ItemName = sheet.Cells[i, 2].StringValue,
                        Method = sheet.Cells[i, 3].StringValue,
                        Standard = sheet.Cells[i, 4].StringValue,
                        DeptName = sheet.Cells[i, 5].StringValue,
                    };
                    importList.Add(import);
                }


                //检查表名称、设备系统、班组相同的数据进行合并
                List<InspectionGroupingModel> groupdata = new List<InspectionGroupingModel>();
                foreach (var item in importList)
                {
                    //判断该数据是否是要合并的数据
                    if (groupdata.Any(x => x.InspectionName.Equals(item.InspectionName) && x.DeivceSystem.Equals(item.DeviceSystem) && x.DeptName.Equals(item.DeptName)))
                    {
                        //合并
                        groupdata.FirstOrDefault(x => x.InspectionName.Equals(item.InspectionName) && x.DeivceSystem.Equals(item.DeviceSystem) && x.DeptName.Equals(item.DeptName)).ItemList.Add(item);
                        continue;
                    }
                    //创建新的合并数据
                    groupdata.Add(new InspectionGroupingModel()
                    {

                        DeivceSystem = item.DeviceSystem,
                        DeptName = item.DeptName,
                        InspectionName = item.InspectionName,
                        ItemList = new List<ImportInspectionModel>() { item }
                    });
                }

                //组装数据 
                if (groupdata != null && groupdata.Count > 0)
                {

                    List<DepartmentEntity> deptList = new DepartmentBLL().GetAll();//所有的部门
                    List<DeviceInspectionEntity> inspectionEntities = new List<DeviceInspectionEntity>();//要新增的检查表的数据
                    List<DeviceInspectionItemEntity> itemEntities = new List<DeviceInspectionItemEntity>();//要新增的检查项的数据
                    groupdata.ForEach(groupItem => {
                        //先组装检查表的数据
                        DepartmentEntity dept = deptList.First(p => p.FullName.Equals(groupItem.DeptName));
                        if (dept == null)
                        {
                            success = false; message = $"班组\"{groupItem.DeptName}\"不存在，请更改后再上传";
                        }
                        DeviceInspectionEntity inspectionEntity = new DeviceInspectionEntity()
                        {
                            InspectionName = groupItem.InspectionName,
                            DeviceSystem = groupItem.DeivceSystem,
                            DeptId = dept.DepartmentId,
                            DeptCode = dept.EnCode,
                            DeptName = dept.FullName
                        };
                        inspectionEntity.Create();
                        inspectionEntities.Add(inspectionEntity);
                        //后组装检查项的数据
                        groupItem.ItemList.ForEach(item => {
                            itemEntities.Add(new DeviceInspectionItemEntity()
                            {
                                Id = Guid.NewGuid().ToString(),
                                DeviceId = inspectionEntity.Id,
                                ItemName = item.ItemName,
                                Method = item.Method,
                                Standard = item.Standard
                            });
                        });
                    });

                    //新增到数据库
                    _bll.Import(inspectionEntities, itemEntities);
                }
                else
                {
                    success = false; message = "导入数据为空";
                }
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
        #endregion
    }
}