using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Web.Mvc;
using System;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.SystemManage;

namespace BSFramework.Application.Web.Areas.SystemManage.Controllers
{
    /// <summary>
    /// 描 述：意见反馈
    /// </summary>
    public class FeedbackController : MvcControllerBase  
    {
        private FeedbackBLL newsBLL = new FeedbackBLL();
        private UserBLL userbll = new UserBLL(); //用户操作对象
        private DataItemDetailBLL dataitemdetailbll = new DataItemDetailBLL();

        #region 视图功能
        /// <summary>
        /// 意见反馈管理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 意见反馈表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        #endregion


        #region 页面组件初始化
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetInitDataJson()
        {
            Operator CreateUser = OperatorProvider.Provider.Current();
            string opinionPhoto = Guid.NewGuid().ToString();
            //返回值
            var josnData = new
            {
                User = CreateUser, //用户对象
                OpinionPhoto = opinionPhoto
            };

            return Content(josnData.ToJson());
        }
        #endregion


        #region 获取数据
        /// <summary>
        /// 意见反馈列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            Operator opertator = new OperatorProvider().Current();
            string OrgCode = opertator.OrganizeCode;
            queryJson = queryJson.Insert(1, "\"OrgCode\":\"" + OrgCode + "\","); //添加当前组织机构
            var data = newsBLL.GetPageList(pagination, queryJson);
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
        /// 意见反馈实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = newsBLL.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除意见反馈
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除意见反馈信息")]
        public ActionResult RemoveForm(string keyValue)
        {
            newsBLL.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存意见反馈表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="opinionEntity">意见反馈实体</param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        [ValidateInput(false)]
        [HandlerMonitor(5, "保存意见反馈表单(新增、修改)")]
        public ActionResult SaveForm(string keyValue, FeedbackEntity opinionEntity)
        {
            newsBLL.SaveForm(keyValue, opinionEntity); 
            return Success("操作成功。");
        }
        #endregion
    }
}
