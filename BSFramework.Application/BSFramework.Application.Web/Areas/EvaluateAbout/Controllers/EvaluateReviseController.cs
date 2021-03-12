using BSFramework.Application.Busines.EvaluateAbout;
using BSFramework.Application.Code;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.EvaluateAbout.Controllers
{
    public class EvaluateReviseController : Controller
    {
        //
        // GET: /EvaluateAbout/EvaluateRevise/

        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            //pagination.p_kid = "Id";
            //pagination.p_fields = "CreateDate,CreateUser,CreateUserId,CategoryId,Category,StandardScore,DepartmentId,DepartmentName,GroupId,GroupName,DeptScore,DeptCause,DeptEvaluateUserId,DeptEvaluteUser,ReviseScore,ReviseCause,ReviseUserId,ReviseUser,EvaluteContentId,EvaluteContent,IsDeleteType,EvaluateId,EvaluateSeason";
            //pagination.p_tablename = "WG_EvaluateRevise";
            //pagination.conditionJson = " 1=1 ";
            //pagination.sidx = "CreateDate";
            //pagination.sord = "desc";
            Operator user = OperatorProvider.Provider.Current();
            var data = new EvaluateReviseBLL().GetPagesList(pagination, queryJson);
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
        /// 绑定下拉搜索框
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult BindCombobox()
        {
           
            var data = new EvaluateReviseBLL().BindCombobox();
            return Content(data.ToJson());
        }

    }
}
