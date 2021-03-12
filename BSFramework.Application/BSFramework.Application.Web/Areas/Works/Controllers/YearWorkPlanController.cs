using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.YearWorkPlanManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.YearWorkPlan;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.Works.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class YearWorkPlanController : MvcControllerBase
    {
        private YearWorkPlanBLL bll = new YearWorkPlanBLL();
        private UserBLL userbll = new UserBLL();
        //
        // GET: /Works/YearWorkPlan/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult From()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();

            decimal total = 0;
            int records = 0;
            pagination.p_kid = "id";
            pagination.p_fields = @"plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE";
            pagination.p_tablename = @"(select id,plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE ";
            pagination.p_tablename +="  from  (select id,plan,remark,planstart,planend,planfinish,lastprogress,progress,bookmark,deptname,deptid,deptcode,CREATEUSERID,CREATEUSERNAME,MODIFYDATE,MODIFYUSERID,MODIFYUSERNAME,CREATEDATE  from wg_yearworkplan  ";
            pagination.sidx = "CREATEDATE";
            pagination.sord = "desc";
            pagination.conditionJson = "1=1";
            var user = OperatorProvider.Provider.Current();
            var myuser = userbll.GetEntity(user.UserId);
            pagination.p_tablename += string.Format(" where deptcode like '{0}%'", myuser.DepartmentCode);
            pagination.p_tablename += " ORDER BY CREATEDATE DESC) t GROUP BY bookmark ) as t ";
            var data = bll.GetPageList(pagination, queryJson);
            var JsonData = new
            {
                rows = data,
                total = total,
                page = pagination.page,
                records = records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 新增和修改
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveFrom(string id, string model)
        {
            var success = true;
            var message = "保存成功";
            var user = OperatorProvider.Provider.Current();
            var myuser = userbll.GetEntity(user.UserId);
            try
            {
                var data = model.ToObject<YearWorkPlanEntity>();
                bll.SaveForm(myuser, data);
            }
            catch (Exception ex)
            {

                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });

        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <returns></returns>
        
        public ActionResult getEntity(string keyValue)
        {
            var data=  bll.GetEntity(keyValue);
            return Content(data.ToJson()); 
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Delete(string keyValue)
        {

            var success = true;
            var message = "删除成功";

            try
            {

                bll.RemoveForm(keyValue);
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
            }

            return Json(new AjaxResult { type = success ? ResultType.success : ResultType.error, message = HttpUtility.JavaScriptStringEncode(message) });
        }


    }
}
