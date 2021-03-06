using BSFramework.Application.Entity.CustomerManage;
using BSFramework.Application.Busines.CustomerManage;
using BSFramework.Util;
using BSFramework.Util.WebControl;
using System.Web.Mvc;
using BSFramework.Application.Code;

namespace BSFramework.Application.Web.Areas.CustomerManage.Controllers
{
    /// <summary>
    /// 描 述：客户信息
    /// </summary>
    public class CustomerController : MvcControllerBase
    {
        private CustomerBLL customerbll = new CustomerBLL();
        private CustomerContactBLL customercontactbll = new CustomerContactBLL();

        #region 视图功能
        /// <summary>
        /// 客户列表页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 客户表单页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 客户明细页面
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult Detail()
        {
            return View();
        }
        /// <summary>
        /// 联系人列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [HandlerAuthorize(PermissionMode.Enforce)]
        public ActionResult ContactIndex()
        {
            return View();
        }
        /// <summary>
        /// 联系人表单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ContactForm()
        {
            return View();
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetListJson()
        {
            var data = customerbll.GetList();
            return ToJsonResult(data);
        }
        /// <summary>
        /// 获取客户列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回分页列表Json</returns>
        [HttpGet]
        public ActionResult GetPageListJson(Pagination pagination, string queryJson)
        {
            var watch = CommonHelper.TimerStart();
            var data = customerbll.GetPageList(pagination, queryJson);
            var jsonData = new
            {
                rows = data,
                total = pagination.total,
                page = pagination.page,
                records = pagination.records,
                costtime = CommonHelper.TimerEnd(watch)
            };
            return ToJsonResult(jsonData);
        }
        /// <summary>
        /// 获取客户实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = customerbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns>返回列表Json</returns>
        [HttpGet]
        public ActionResult GetContactListJson(string queryJson)
        {
            var data = customercontactbll.GetList(queryJson);
            return ToJsonResult(data);
        }
        /// <summary>
        /// 获取联系人实体 
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns>返回对象Json</returns>
        [HttpGet]
        public ActionResult GetContactFormJson(string keyValue)
        {
            var data = customercontactbll.GetEntity(keyValue);
            return ToJsonResult(data);
        }
        #endregion

        #region 验证数据
        /// <summary>
        /// 客户名称不能重复
        /// </summary>
        /// <param name="FullName">名称</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ExistFullName(string FullName, string keyValue)
        {
            bool IsOk = customerbll.ExistFullName(FullName, keyValue);
            return Content(IsOk.ToString());
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除客户数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerAuthorize(PermissionMode.Enforce)]
        [HandlerMonitor(6, "删除客户信息")]
        public ActionResult RemoveForm(string keyValue)
        {
            customerbll.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存客户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)客户信息")]
        public ActionResult SaveForm(string keyValue, CustomerEntity entity)
        {
            customerbll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        /// <summary>
        /// 删除联系人数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(6, "删除联系人信息")]
        public ActionResult RemoveContactForm(string keyValue)
        {
            customercontactbll.RemoveForm(keyValue);
            return Success("删除成功。");
        }
        /// <summary>
        /// 保存联系人表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [HandlerMonitor(5, "保存(新增或修改)联系人信息")]
        public ActionResult SaveContactForm(string keyValue, CustomerContactEntity entity)
        {
            customercontactbll.SaveForm(keyValue, entity);
            return Success("操作成功。");
        }
        #endregion
    }
}
