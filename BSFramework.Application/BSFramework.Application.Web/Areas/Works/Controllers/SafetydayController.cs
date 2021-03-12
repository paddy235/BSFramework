using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.QuestionManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Web.Areas.Works.Models;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    public class SafetydayController : MvcControllerBase
    {
        private SafetydayBLL safetydaybll = new SafetydayBLL();
        private DepartmentBLL deptbll = new DepartmentBLL();


        #region 视图功能
        /// <summary>
        /// 列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Form()
        {
            var user = OperatorProvider.Provider.Current();
            //var tree = dept.GetAuthorizationDepartment(user.DeptId);
            var deptid = deptbll.GetRootDepartment();
            ViewBag.deptid = deptid.DepartmentId;
            return View();
        }
        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Detail()
        {
            return View();
        }

        /// <summary>
        /// 表单页面(查看)
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult material()
        {
            return View();
        }

        /// <summary>
        /// 首页月度安全日活动分布页
        /// </summary>
        /// <returns></returns>
        public ActionResult IndexSafetydayPartView()
        {
            Operator user = OperatorProvider.Provider.Current();
            var company = deptbll.GetCompany(user.DeptId);//获取当前登录人所在的电厂
            var subDept = deptbll.GetSubGroups(company.DepartmentId);//获取该电厂下所有的班组
            DateTime now = DateTime.Now;

            var data = safetydaybll.GetStatistics(subDept.Select(p => p.DepartmentId), new DateTime(now.Year, now.Month, 1), now.Date,"安全日活动");
            return View(data);
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public JsonResult GetListJson(string name, int rows, int page)
        {
            var total = 0;
            var data = safetydaybll.GetList(name, rows, page, out total);

            return Json(new { rows = data, page = page, total = Math.Ceiling((decimal)total / rows) }, JsonRequestBehavior.AllowGet);




            //pagination.p_kid = "Id";
            //pagination.p_fields = "CreateUserName,CreateDate,Subject,DeptName,activitytype,deptid";
            //pagination.p_tablename = "wg_safetyday";
            //pagination.conditionJson = "1=1";
            //var watch = CommonHelper.TimerStart();


            //var data = safetydaybll.GetPageList(pagination, queryJson);
            //data.Columns.Add("readnum", Type.GetType("System.String"));
            //for (int i = 0; i < data.Rows.Count; i++)
            //{
            //    var readnum = string.Empty;
            //    var deptStr = data.Rows[i]["deptid"].ToString();
            //    var keyvalue = data.Rows[i]["Id"].ToString();
            //    var deptList = deptStr.Split(',');
            //    var isreadnum = 0;
            //    var deptnum = deptList.Length;
            //    for (int j = 0; j < deptList.Length; j++)
            //    {
            //        var read = safetydaybll.getMaterial(deptList[j], keyvalue);
            //        if (read.Count > 0)
            //        {
            //            var readGroup = read.GroupBy(x => x.deptname);
            //            foreach (var item in readGroup)
            //            {
            //                // var deptreadnum = item.Count();
            //                var deptisread = item.Where(x => x.isread).Count() > 0;
            //                if (deptisread)
            //                {
            //                    isreadnum++;
            //                }
            //            }
            //        }
            //        else
            //        {
            //            deptnum = 0;
            //            break;
            //        }
            //    }
            //    if (deptnum == 0)
            //    {
            //        readnum = "*";
            //    }
            //    else
            //    {
            //        readnum = isreadnum + "/" + deptnum;
            //    }

            //    data.Rows[i]["readnum"] = readnum;
            //}
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
        /// 获取列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult getDeptRead(string keyvalue)
        {
            var watch = CommonHelper.TimerStart();
            var entity = safetydaybll.GetEntity(keyvalue);
            var data = new DataTable();
            var deptList = entity.DeptId.Split(',');
            data.Columns.Add("deptname", Type.GetType("System.String"));
            data.Columns.Add("readnum", Type.GetType("System.String"));
            DataRow dr;
            for (int i = 0; i < deptList.Length; i++)
            {
                var read = safetydaybll.getMaterial(deptList[i], keyvalue);


                if (read.Count > 0)
                {
                    var readGroup = read.GroupBy(x => x.deptname);
                    foreach (var item in readGroup)
                    {
                        dr = data.NewRow();
                        dr["deptname"] = item.Key;
                        var deptreadnum = item.Count();
                        var deptisread = item.Where(x => x.isread).Count();
                        if (deptisread > 0)
                        {
                            dr["readnum"] = "已学习" + deptisread + "/" + deptreadnum;
                        }
                        else
                        {
                            dr["readnum"] = "未学习";

                        }
                        data.Rows.Add(dr);
                    }
                }
            }
            var JsonData = new
            {
                rows = data,
                total = data.Rows.Count,
                page = 1,
                records = data.Rows.Count,
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
            var data = safetydaybll.GetEntity(keyValue);
            FileInfoBLL fi = new FileInfoBLL();
            IList list = fi.GetFilesByRecId(keyValue);
            var json = JsonConvert.SerializeObject(list);
            List<FileInfoModel> fileList = JsonConvert.DeserializeObject<List<FileInfoModel>>(json);
            fileList.ForEach(x =>
            {
                if (x.FileExtensions.ToLower().Contains("gw"))
                {
                    x.FilePath = x.FilePath.ToLower().Replace(".gw", ".pdf");
                }
            });
            return ToJsonResult(new { formData = data, files = fileList });
        }


        /// <summary>
        /// 获取附件信息 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFileListJson(string keyValue)
        {
            var data = safetydaybll.GetEntity(keyValue);
            FileInfoBLL fi = new FileInfoBLL();
            var list = fi.GetFilesByRecIdNew(keyValue);
            return ToJsonResult(new { formData = data, files = list });
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
            safetydaybll.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="fileName">附件名称</param>
        /// <param name="recId">业务记录Id</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveFile(string fileName, string recId)
        {
            FileInfoBLL fileBll = new FileInfoBLL();
            FileInfoEntity entity = fileBll.GetEntity(recId, fileName);
            QuestionBankBLL question = new QuestionBankBLL();
            try
            {
                if (entity != null)
                {
                    //res=fileBll.DeleteFile(recId,fileName,Server.MapPath(entity.FilePath));
                    fileBll.Delete(entity.FileId);
                    safetydaybll.RemoveFile(entity.FileId);
                    #region 清理试题下的fileid
                    question.DetailbyFileId(entity.FileId);
                    #endregion
                }
                return Success("操作成功。");
            }
            catch (Exception)
            {

                return Error("操作失败");
            }

        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, SafetydayEntity entity)
        {
            safetydaybll.SaveForm(keyValue, entity, Server.MapPath("~"));

            var messagebll = new MessageBLL();
            messagebll.SendMessage("活动材料下发", keyValue);

            return Success("操作成功。");
        }
        #endregion
    }
}
