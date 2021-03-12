using BSFramework.Application.Busines.FlowManage;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.FlowManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.FlowManage.Controllers
{
    /// <summary>
    /// 描 述：工作流草稿流程
    /// </summary>
    public class FlowRoughdraftController : MvcControllerBase
    {
        private WFRuntimeBLL wfProcessBll = new WFRuntimeBLL();
        #region 视图功能
        // GET: /FlowManage/FlowRoughdraft/
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 创建实例
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FlowProcessBuider()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 草稿列表(分页)
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {

            var user = OperatorProvider.Provider.Current();

            pagination.page++;
            var data = wfProcessBll.GetPageList(user.UserId, pagination, queryJson, "3");
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
            };
            return Content(JsonData.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 编辑草稿流程实例
        /// </summary>
        /// <param name="keyVlaue">流程模板信息Id</param>
        /// <param name="frmData">表单数据</param>
        /// <param name="type">1发起，3草稿</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)草稿流程实例信息")]
        public ActionResult EditionRoughdraftProcess(string keyValue, string wfProcessInstanceJson, string frmData)
        {
            var user = OperatorProvider.Provider.Current();

            WFProcessInstanceEntity wfProcessInstanceEntity = wfProcessInstanceJson.ToObject<WFProcessInstanceEntity>();
            wfProcessBll.EditionRoughdraftProcess(user.UserId, user.UserName, keyValue, wfProcessInstanceEntity, frmData);
            string text = "创建成功";
            if (wfProcessInstanceEntity.EnabledMark != 1)
            {
                text = "草稿保存成功";
            }
            return Success(text);
        }
        #endregion

    }
}
