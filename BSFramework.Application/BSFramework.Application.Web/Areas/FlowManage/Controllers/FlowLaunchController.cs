using BSFramework.Application.Busines.FlowManage;
using BSFramework.Application.Entity.FlowManage;
using System;
using System.Collections.Generic;
using BSFramework.Util;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BSFramework.Application.Code;

namespace BSFramework.Application.Web.Areas.FlowManage.Controllers
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：流程发起
    /// </summary>
    public class FlowLaunchController : MvcControllerBase
    {
        private WFRuntimeBLL wfProcessBll = new WFRuntimeBLL();
        #region 视图功能
        //
        // GET: /FlowManage/FlowLaunch/
        /// <summary>
        /// 管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 预览
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult PreviewIndex()
        {
            return View();
        }
        /// <summary>
        /// 创建流程实例
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FlowProcessNewForm()
        {
            return View();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 创建流程实例
        /// </summary>
        /// <param name="wfSchemeInfoId">流程模板信息Id</param>
        /// <param name="frmData">表单数据</param>
        /// <param name="type">0发起，3草稿</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)流程实例信息")]
        public ActionResult CreateProcess(string wfSchemeInfoId, string wfProcessInstanceJson, string frmData)
        {
            var user = OperatorProvider.Provider.Current();
            WFProcessInstanceEntity wfProcessInstanceEntity = wfProcessInstanceJson.ToObject<WFProcessInstanceEntity>();
            wfProcessBll.CreateProcess(user.UserId, user.UserName, wfSchemeInfoId, wfProcessInstanceEntity, frmData);
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
