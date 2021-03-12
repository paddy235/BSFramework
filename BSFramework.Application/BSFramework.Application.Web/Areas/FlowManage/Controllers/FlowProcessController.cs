using BSFramework.Application.Busines.FlowManage;
using BSFramework.Application.Code;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Web.Mvc;


namespace BSFramework.Application.Web.Areas.FlowManage.Controllers
{
    /// <summary>
    /// 描 述:流程实例公用处理方法
    /// </summary>
    public class FlowProcessController : MvcControllerBase
    {
        private WFRuntimeBLL wfProcessBll = new WFRuntimeBLL();
        #region 视图功能
        /// <summary>
        /// 流程监控
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult MonitoringIndex()
        {
            return View();
        }
        /// <summary>
        /// 流程指派
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DesignationIndex()
        {
            return View();
        }
        /// <summary>
        /// 流程进度查看
        /// </summary>
        /// <returns></returns>\
        [HttpGet]
        public ActionResult ProcessLookFrom()
        {
            return View();
        }
        /// <summary>
        /// 流程指派
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ProcessDesignate()
        {
            return View();
        }
        #endregion

        #region 获取数据(公用)
        /// <summary>
        /// 工作流列表,流程监控用(分页)
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetProcessPageListJson(Pagination pagination, string queryJson)
        {
            var data = wfProcessBll.GetPageList(pagination, queryJson);
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 工作流列表,运行中(分页)
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetRuntimePageListJson(Pagination pagination, string queryJson)
        {
            var user = OperatorProvider.Provider.Current();
            pagination.page++;
            var data = wfProcessBll.GetPageList(user.UserId, pagination, queryJson, "1");
            var JsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取进程模板Json
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProcessSchemeJson(string keyValue)
        {
            var data = wfProcessBll.GetProcessSchemeEntity(keyValue);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 已办流程进度查看，根据当前访问人的权限查看表单内容
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProcessSchemeEntityByUserId(string keyValue)
        {
            var data = wfProcessBll.GetProcessSchemeEntityByUserId(keyValue);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 已办流程进度查看，根据当前节点的权限查看表单内容
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="isPermission"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProcessSchemeEntityByNodeId(string keyValue, string nodeId)
        {
            var data = wfProcessBll.GetProcessSchemeEntityByNodeId(keyValue, nodeId);
            return Content(data.ToJson());
        }
        /// <summary>
        /// 获取进程信息
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProcessInfoJson(string keyValue)
        {
            var processInstance = wfProcessBll.GetProcessInstanceEntity(keyValue);
            var processScheme = wfProcessBll.GetProcessSchemeEntity(processInstance.ProcessSchemeId);
            var JsonData = new
            {
                processInstance = processInstance,
                processScheme = processScheme
            };
            return Content(JsonData.ToJson());
        }
        /// <summary>
        /// 获取进程实例
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetProcessInstanceJson(string keyValue)
        {
            var processInstance = wfProcessBll.GetProcessInstanceEntity(keyValue);
            return Content(processInstance.ToJson());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除工作流实例进程
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "删除工作流实例进程信息")]
        public ActionResult DeleteProcess(string keyValue)
        {
            wfProcessBll.DeleteProcess(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 删除工作流实例进程
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(0, "虚拟操作实例(0暂停;1启用;2取消/召回)")]
        public ActionResult DeleteVirtualProcess(string keyValue)
        {
            var user = OperatorProvider.Provider.Current();

            wfProcessBll.OperateVirtualProcess(user.UserName, keyValue, 2);
            return Success("召回成功。");
        }
        /// <summary>
        /// 删除工作流实例进程
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(0, "虚拟操作实例(0暂停;1启用;2取消/召回)")]
        public ActionResult OperateProcess(string keyValue, int state)
        {
            var user = OperatorProvider.Provider.Current();

            wfProcessBll.OperateVirtualProcess(user.UserName, keyValue, state);
            return Success("操作成功。");
        }
        /// <summary>
        /// 审核流程
        /// </summary>
        /// <param name="processId">工作流实例主键Id</param>
        /// <param name="processId">审核数据</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(0, "审核流程")]
        public ActionResult VerificationProcess(string processId, string verificationData)
        {
            var user = OperatorProvider.Provider.Current();

            wfProcessBll.VerificationProcess(user.UserId, user.UserName, processId, verificationData);
            return Success("审核成功。");
        }
        /// <summary>
        /// 指派流程
        /// </summary>
        /// <param name="processId">工作流实例主键Id</param>
        /// <param name="processId">审核数据</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(0, "指派流程")]
        public ActionResult DesignateProcess(string processId, string makeLists)
        {
            wfProcessBll.DesignateProcess(processId, makeLists);
            return Success("指派成功。");
        }
        #endregion
    }
}
