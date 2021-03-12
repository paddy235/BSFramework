using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.CarcOrCardManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.CarcOrCardManage;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// Carc 手袋卡
    /// </summary>
    public class CarcOrCardController : MvcControllerBase
    {
        /// <summary>
        /// 管理平台台账
        /// </summary>
        /// <returns></returns>
        public ActionResult CarcIndex()
        {
            var user = OperatorProvider.Provider.Current();
            var useuser = new UserBLL().GetEntity(user.UserId);
            DepartmentBLL deptbll = new DepartmentBLL();

            if (user.DeptId == "0")
            {
                var tree = deptbll.GetAuthorizationDepartment(user.DeptId);
                ViewBag.DeptId = tree.DepartmentId;
            }
            else
            {
                ViewBag.DeptId = user.DeptId;
            }
            if (useuser.RoleName.Contains("厂级部门用户"))
            {
                ViewBag.DeptId = "";
            }
            ViewBag.username = user.UserName;
            return View();
        }
        /// <summary>
        /// 管理平台台账
        /// </summary>
        /// <returns></returns>
        public ActionResult CardIndex()
        {
            var user = OperatorProvider.Provider.Current();
            ViewBag.userid = user.UserId;
            return View();
        }
        /// <summary>
        ///风险因素
        /// </summary>
        /// <returns></returns>
        public ActionResult DangerList()
        {
            return View();
        }
        /// <summary>
        ///carc card
        /// </summary>
        /// <returns></returns>
        public ActionResult CCardDetail()
        {
            return View();
        }


        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        public ActionResult Import()
        {
            return View();
        }
        /// <summary>
        /// carc  详情 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult CarcDetail()
        {
            return View();
        }
        /// <summary>
        /// 手袋卡  详情 修改
        /// </summary>
        /// <returns></returns>
        public ActionResult CardDetail()
        {
            return View();
        }
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        public ActionResult GetCarcPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //var queryParam = queryJson.ToJObject();
            var Bll = new CarcOrCardBLL();
            var user = OperatorProvider.Provider.Current();
            var data = Bll.GetPageList(pagination, queryJson, user.UserId);
            var jsonStr = JsonConvert.SerializeObject(data);
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
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult getEntity(string keyvalue)
        {
            var Bll = new CarcOrCardBLL();
            var data = Bll.GetDetail(keyvalue);
            foreach (var item in data.Files)
            {
                if (!string.IsNullOrEmpty(item.FilePath))
                    item.FilePath = string.Format("{0}://{1}{2}", this.Request.Url.Scheme, this.Request.Url.Host, Url.Content(item.FilePath));
            }
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="keyvalue"></param>
        /// <returns></returns>
        public ActionResult getCEntity(string keyvalue)
        {
            var Bll = new CarcOrCardBLL();
            var data = Bll.GetCDetail(keyvalue);
            return Content(data.ToJson());
        }


        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>   
        public ActionResult GetCardPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //var queryParam = queryJson.ToJObject();
            var Bll = new CarcOrCardBLL();
            var user = OperatorProvider.Provider.Current();
            var data = Bll.GetCPageList(pagination, queryJson, user.UserId);
            var jsonStr = JsonConvert.SerializeObject(data);
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

        #region 操作
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult CSaveForm(CCardEntity entity)
        {
            try
            {
                var user = OperatorProvider.Provider.Current();
                var Bll = new CarcOrCardBLL();
                var list = new List<CCardEntity>();
                list.Add(entity);
                Bll.CSaveForm(list, user.UserId);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]

        public ActionResult CdelEntity(string keyvalue)
        {
            try
            {
                var Bll = new CarcOrCardBLL();
                Bll.deleteCEntity(keyvalue);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost]

        public ActionResult delEntity(string keyvalue)
        {
            try
            {
                var Bll = new CarcOrCardBLL();
                Bll.deleteEntity(keyvalue);
                return Success("操作成功。");

            }
            catch (Exception ex)
            {

                return Error(ex.Message);
            }

        }
        #endregion

        #region 导入
        /// <summary>
        /// 导入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoImport()
        {
            var success = true;
            var message = "保存成功！";
            var user = OperatorProvider.Provider.Current();

            if (this.Request.Files.Count > 0)
            {
                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];
                var Bll = new CarcOrCardBLL();
                try
                {
                    var titleIndex = this.GetTitleRow(sheet);
                    var data = this.GetData(sheet, titleIndex, user.UserId);
                    Bll.CSaveForm(data, user.UserId);
                }
                catch (Exception e)
                {
                    success = false;
                    message = e.Message;
                }

            }

            return Json(new AjaxResult() { type = success ? ResultType.success : ResultType.error, message = message });
        }
        private List<CCardEntity> GetData(Worksheet sheet, int titleIndex, string userid)
        {
            var userbll = new UserBLL();
            var user = userbll.GetEntity(userid);
            var deptBll = new DepartmentBLL();
            var allDept = deptBll.GetAll();
            var total = 0;
            var deptuser = userbll.GetList(user.DepartmentId, 10000, 1, out total);
            var result = new List<CCardEntity>();
            for (int i = titleIndex + 1; i <= sheet.Cells.MaxDataRow; i++)
            {

                var workname = sheet.Cells[i, 1].StringValue;
                if (string.IsNullOrEmpty(workname))
                {
                    throw new Exception(string.Format("行 {0} 工作任务为空！", i + 1));
                }

                var deptList = sheet.Cells[i, 2].StringValue;

                if (string.IsNullOrEmpty(deptList))
                {
                    throw new Exception(string.Format("行 {0} 部门为空！", i + 1));
                }
                foreach (var item in deptList.Split(','))
                {
                    if (string.IsNullOrEmpty(item))
                    {
                        continue;
                    }
                    var entity = new CCardEntity();
                    entity.WorkName = workname;
                    var ck = allDept.FirstOrDefault(x => x.FullName == item);
                    if (ck == null)
                    {
                        throw new Exception(string.Format("行 {0} 部门不存在！", i + 1));
                    }
                    var getEntity = result.FirstOrDefault(x => x.WorkName == entity.WorkName && x.DeptName == item);
                    if (getEntity == null)
                    {
                        entity.DeptId = ck.DepartmentId;
                        entity.DeptCode = ck.EnCode;
                        entity.DeptName = ck.FullName;
                        #region 
                        var job = getNewData(userid, entity.DeptId);
                        var DutyNameList = sheet.Cells[i, 3].StringValue;
                        if (string.IsNullOrEmpty(DutyNameList))
                        {
                            throw new Exception(string.Format("行 {0} 岗位为空！", i + 1));
                        }
                        var DutyId = string.Empty;
                        foreach (var duty in DutyNameList.Split(','))
                        {
                            if (string.IsNullOrEmpty(duty))
                            {
                                continue;
                            }
                            var ckduty = job.FirstOrDefault(x => x.FullName == duty);
                            if (ckduty == null)
                            {
                                throw new Exception(string.Format("行 {0} 岗位不存在！", i + 1));
                            }
                            DutyId += ckduty.RoleId + ",";
                        }
                        DutyId = DutyId.Substring(0, DutyId.Length - 1);
                        entity.DutyId = DutyId;
                        entity.DutyName = DutyNameList;
                        var WorkArea = sheet.Cells[i, 4].StringValue;
                        //if (string.IsNullOrEmpty(WorkArea))
                        //{
                        //    throw new Exception(string.Format("行 {0} 区域为空！", i + 1));
                        //}
                        entity.WorkArea = WorkArea;
                        var MainOperation = sheet.Cells[i, 5].StringValue;
                        if (string.IsNullOrEmpty(MainOperation))
                        {
                            throw new Exception(string.Format("行 {0} 主要步骤为空！", i + 1));
                        }
                        entity.MainOperation = MainOperation;
                        var dangers = new CDangerousEntity();
                        var DangerName = sheet.Cells[i, 6].StringValue;
                        if (string.IsNullOrEmpty(DangerName))
                        {
                            throw new Exception(string.Format("行 {0} 危害名称为空！", i + 1));
                        }
                        dangers.DangerName = DangerName;
                        var DangerSource = sheet.Cells[i, 7].StringValue;
                        if (string.IsNullOrEmpty(DangerName))
                        {
                            throw new Exception(string.Format("行 {0} 风险描述为空！", i + 1));
                        }
                        dangers.DangerSource = DangerSource;
                        var Measurelist = sheet.Cells[i, 8].StringValue;
                        if (string.IsNullOrEmpty(Measurelist))
                        {
                            throw new Exception(string.Format("行 {0} 采取的控制措施为空！", i + 1));
                        }
                        dangers.Measure = new List<CMeasureEntity>();
                        foreach (var Measures in Measurelist.Split('\n'))
                        {
                            if (string.IsNullOrEmpty(Measures))
                            {
                                continue;
                            }
                            var Measure = new CMeasureEntity();
                            Measure.Measure = Measures;
                            dangers.Measure.Add(Measure);
                        }
                        entity.CDangerousList = new List<CDangerousEntity>();
                        entity.CDangerousList.Add(dangers);

                        #endregion
                        result.Add(entity);
                    }
                    else
                    {
                        var dangers = new CDangerousEntity();
                        var DangerName = sheet.Cells[i, 6].StringValue;
                        if (string.IsNullOrEmpty(DangerName))
                        {
                            throw new Exception(string.Format("行 {0} 危害名称为空！", i + 1));
                        }
                        dangers.DangerName = DangerName;
                        var DangerSource = sheet.Cells[i, 7].StringValue;
                        if (string.IsNullOrEmpty(DangerName))
                        {
                            throw new Exception(string.Format("行 {0} 风险描述为空！", i + 1));
                        }
                        dangers.DangerSource = DangerSource;
                        var Measurelist = sheet.Cells[i, 8].StringValue;
                        if (string.IsNullOrEmpty(Measurelist))
                        {
                            throw new Exception(string.Format("行 {0} 采取的控制措施为空！", i + 1));
                        }
                        dangers.Measure = new List<CMeasureEntity>();
                        foreach (var Measures in Measurelist.Split('\n'))
                        {
                            if (string.IsNullOrEmpty(Measures))
                            {
                                continue;
                            }
                            var Measure = new CMeasureEntity();
                            Measure.Measure = Measures;
                            dangers.Measure.Add(Measure);
                        }

                        getEntity.CDangerousList.Add(dangers);
                    }
                }


            }

            return result;
        }
        /// <summary>
        /// 获取双控岗位
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="deptid"></param>
        /// <returns></returns>
        public List<RoleEntity> getNewData(string userid, string deptid)
        {
            if (deptid == "0") deptid = "";
            var dict = new
            {
                data = deptid,
                userid = userid,
                tokenid = ""
            };
            string res = HttpMethods.HttpPost(Path.Combine(Config.GetValue("ErchtmsApiUrl"), "Post", "GetPostByDeptId"), "json=" + JsonConvert.SerializeObject(dict));
            var ret = JsonConvert.DeserializeObject<RetDataModel>(res);
            return JsonConvert.DeserializeObject<List<RoleEntity>>(ret.data.ToString());
            //return new List<RoleEntity>();
        }
        private int GetTitleRow(Worksheet sheet)
        {
            for (int i = 0; i <= sheet.Cells.MaxDataRow; i++)
            {
                if (sheet.Cells[i, 0].StringValue == "序号") return i;
            }

            throw new Exception("无法识别文件！");
        }
        #endregion
    }
}