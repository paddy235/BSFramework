using BSFramework.Application.Busines.SweepManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.SweepManage;
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
    /// 保洁管理
    /// </summary>
    public class SweepController : MvcControllerBase
    {

        private SweepBLL _bll = new SweepBLL();
        /// <summary>
        /// 保洁管理
        /// </summary>
        /// <returns></returns>
        // GET: Works/Sweep
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }
        #region 查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetPageSweepList(Pagination pagination, string queryJson)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                Operator user = OperatorProvider.Provider.Current();
                var data = _bll.GetPageSweepList(pagination, queryJson);
                var JsonData = new
                {
                    rows = data,
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = CommonHelper.TimerEnd(watch)
                };
                return ToJsonResult(JsonData);
            }
            catch (Exception ex)
            {
                var JsonData = new
                {
                    rows = new List<SweepEntity>(),
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = "0",
                    ex.Message
                };
                return ToJsonResult(JsonData);
            }
        }


        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetDetail(string keyValue)
        {

            try
            {
                var entity = _bll.getSweepAndItemDataById(keyValue);
                return ToJsonResult(entity);
            }
            catch (Exception ex)
            {
                return ToJsonResult(ex);
            }
        }
        #endregion
    }
}