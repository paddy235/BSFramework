using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.SystemManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    public class WorkStandardController : MvcControllerBase
    {
        private WorkStandardBLL workStandardBLL = new WorkStandardBLL();
        #region 视图页
        /// <summary>
        /// 工作标准主页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 工作标准详情页
        /// </summary>
        /// <returns></returns>
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 附件显示
        /// </summary>
        /// <param name="recid">主键Id</param>
        /// <returns></returns>
        public ActionResult ShowFile(string recid)
        {
            List<FileInfoEntity> fileList = new FileInfoBLL().GetFilesByRecIdNew(recid).ToList();
            fileList.ForEach(x =>
            {
                x.FilePath = Url.Content(x.FilePath);
            });
            return View(fileList);
        }
        #endregion

        #region Ajax
        /// <summary>
        /// 获取工作标准列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //pagination.p_kid = "Id";
            //pagination.p_fields = "ModuleId,ModuleCode,ModuleName,Content";
            //pagination.p_tablename = "WG_WorkStandard";
            //pagination.conditionJson = " 1=1 ";
            //pagination.sidx = "CreateTime";
            //pagination.sord = "asc";
            Operator user = OperatorProvider.Provider.Current();
            var data = workStandardBLL.GetPageList(pagination, queryJson);
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
            WorkStandardEntity entity = workStandardBLL.GetEntity(keyValue);
            return ToJsonResult(entity);
        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">接收参数</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, WorkStandardEntity entity)
        {
            try
            {
                workStandardBLL.SaveForm(keyValue, entity);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
          
        }
        /// <summary>
        /// 删除单条数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult RemoveForm(string keyValue)
        {
            try
            {
                workStandardBLL.RemoveForm(keyValue);
                return Success("操作成功。");
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }
          
        }
        
        #endregion
    }
}
