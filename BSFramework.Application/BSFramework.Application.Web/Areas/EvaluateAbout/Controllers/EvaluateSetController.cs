using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Entity.EvaluateAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Controllers
{
    /// <summary>
    /// 班组分类管理
    /// </summary>
    public class EvaluateSetController : MvcControllerBase
    {
        private EvaluateSetBLL safetydaybll = new EvaluateSetBLL();
        private DepartmentBLL dept = new DepartmentBLL();

        #region 视图功能
        /// <summary>
        /// 列表视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 查看视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Detail()
        {
            return View();
        }
        
        #endregion

        #region 获取数据

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(Pagination pagination, string queryJson)
        {
            //pagination.p_kid = "Id";
            //pagination.p_fields = "ClassName,createdate,IsFiring,DeptName,Remark";
            //pagination.p_tablename = "wg_EvaluateSet";
            //pagination.conditionJson = "1=1";
            var watch = CommonHelper.TimerStart();
            var data = safetydaybll.GetPagesList(pagination, queryJson);
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
            var data = safetydaybll.GetEntity(keyValue);
            FileInfoBLL fi = new FileInfoBLL();
            IList list = fi.GetFilesByRecId(keyValue);
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
            int res = 0;
            if (entity != null)
            {
                res = fileBll.DeleteFile(recId, fileName, Server.MapPath(entity.FilePath));
            }
            return res > 0 ? Success("操作成功。") : Error("操作失败");
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
        public ActionResult SaveForm(string keyValue, EvaluateSetEntity entity)
        {
            safetydaybll.SaveForm(keyValue, entity);



            return Success("操作成功。");
        }
        #endregion

    }
}
