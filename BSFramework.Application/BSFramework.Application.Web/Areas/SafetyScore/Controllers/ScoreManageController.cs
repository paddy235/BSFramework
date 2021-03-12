using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SafetyScore;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Application.Entity.WorkMeeting.ViewModel;
using BSFramework.Util;
using BSFramework.Util.Extension;
using BSFramework.Util.Log;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SafetyScore.Controllers
{
    /// <summary>
    /// 积分管理
    /// </summary>
    public class ScoreManageController : MvcControllerBase
    {
        private readonly SafetyScoreBLL _bll;
        private readonly UserBLL _userbll;
        public ScoreManageController()
        {
            _bll = new SafetyScoreBLL();
            _userbll = new UserBLL();
        }

        #region 页面
        /// <summary>
        /// 台账页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var user = OperatorProvider.Provider.Current();
            var thisUser = new UserBLL().GetEntity(user.UserId);
            ViewBag.UserRole = thisUser.RoleName ?? "";
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptCode = dept.EnCode;
            ViewBag.UserDeptCode = dept.EnCode;
            return View();
        }
        /// <summary>
        /// 表单页面 
        /// </summary>
        /// <returns></returns>
        public ActionResult Form()
        {
            var user = OperatorProvider.Provider.Current();
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.dept = dept.DepartmentId;
            ViewBag.UserDeptCode = dept.EnCode;
            return View();
        }

        #region 积分统计
        public ActionResult ScoreIndex()
        {
            var user = OperatorProvider.Provider.Current();
            var thisUser = new UserBLL().GetEntity(user.UserId);
            ViewBag.UserRole = thisUser.RoleName;
            var dept = new DepartmentBLL().GetAuthorizationDepartment(user.DeptId);
            ViewBag.deptid = dept.DepartmentId;
            ViewBag.deptCode = dept.EnCode;
            ViewBag.UserDeptCode = dept.EnCode;
            return View();
        }

        /// <summary>
        /// 用户的年度积分数据
        /// </summary>
        /// <param name="keyValue">UserId</param>
        /// <returns></returns>
        public ActionResult ScoreForm(string keyValue)
        {
            Dictionary<int, List<KeyValue>> dic = _bll.GetUserScoreInfo(keyValue);
            var user = new UserBLL().GetUserInfoEntity(keyValue);
            ViewBag.User = user;
            return View(dic);
        }

        /// <summary>
        /// 用户个人得分详情
        /// </summary>
        /// <param name="userId">用户详情</param>
        /// <param name="searchDate">查询的日期</param>
        /// <param name="searchType">查询类型</param>
        /// <returns></returns>
        public ViewResult UserScoreDetail(string userId, DateTime searchDate, string searchType)
        {
            string quarter = string.Empty;
            switch (searchDate.Month)
            {
                case 1:
                case 2:
                case 3:
                    quarter = "第一季度";
                    break;
                case 4:
                case 5:
                case 6:
                    quarter = "第二季度";
                    break;
                case 7:
                case 8:
                case 9:
                    quarter = "第三季度";
                    break;
                case 10:
                case 11:
                case 12:
                    quarter = "第四季度";
                    break;
            }
            DateTime startDate = searchDate.AddDays(-searchDate.Day+1);
            DateTime endDate = searchDate.AddMonths(1);
            switch (searchType)
            {
                case "季度":
                    startDate = searchDate.AddMonths(0 - (searchDate.Month - 1) % 3).AddDays(1 - searchDate.Day);
                    endDate = startDate.AddMonths(3);
                    break;
                case "年度":
                    startDate = new DateTime(searchDate.Year, 1, 1);
                    endDate = startDate.AddYears(1);
                    break;
            }
            ViewBag.quarter = quarter;//季度
            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;
            return View();
        }
        #endregion
        #endregion


        #region 方法
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="pagination">分页信息</param>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public ActionResult GetPagedList(Pagination pagination, string queryJson)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                Operator user = OperatorProvider.Provider.Current();
                IEnumerable<SafetyScoreEntity> data = _bll.GetPagedList(pagination, queryJson);
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
                    rows = new List<SafetyScoreEntity>(),
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
        /// 获取单个实体
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public ActionResult GetFormJson(string keyValue)
        {
            try
            {
                SafetyScoreEntity entity = _bll.GetEntity(keyValue);
                return ToJsonResult(entity);
            }
            catch (Exception ex)
            {
                return ToJsonResult(ex);
            }
        }
        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult SaveForm(string keyValue, SafetyScoreEntity entity)
        {
            Operator user = OperatorProvider.Provider.Current();
            WriteLog.AddLog($"用户：{user.UserName}  id：{user.UserId}开始编辑积分管理数据{keyValue} \r\n {JsonConvert.SerializeObject(entity)}", "SafetyScore");
            try
            {
                _bll.SaveForm(keyValue, entity);
                return Success("操作成功");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"错误 \r\n用户：{user.UserName}  id：{user.UserId}编辑积分管理数据{keyValue}失败 \r\n 错误信息：{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return Error(ex.Message);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        public ActionResult Remove(string keyValue)
        {
            Operator user = OperatorProvider.Provider.Current();
            WriteLog.AddLog($"用户：{user.UserName}  id：{user.UserId}开始删除积分管理数据{keyValue}", "SafetyScore");
            try
            {
                _bll.Remove(keyValue);
                return Success("删除成功。");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"错误 \r\n用户：{user.UserName}  id：{user.UserId}删除积分管理数据{keyValue}失败 \r\n 错误信息：{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return Error("删除失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 个人分数数据
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="queryJson"></param>
        /// <returns></returns>
        public ActionResult GetUserScorePagedList(Pagination pagination, DateTime serachDate,string keyWord,string deptId,int? Gender)
        {
            try
            {
                var watch = CommonHelper.TimerStart();
                Operator user = OperatorProvider.Provider.Current();
                var subDeptIds = new DepartmentBLL().GetSubDepartments(new string[] { deptId }).Select(x => x.DepartmentId);
                Expression<Func<UserEntity, bool>> userWhere = p => subDeptIds.Contains(p.DepartmentId);

                if (!string.IsNullOrWhiteSpace(keyWord)) userWhere = userWhere.And(x => x.RealName.Contains(keyWord) || x.DutyName.Contains(keyWord));
                if (Gender.HasValue) userWhere = userWhere.And(x => x.Gender==Gender);



                var data = _bll.GetUserScorePagedList(pagination, serachDate, userWhere);
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
                    rows = new List<SafetyScoreEntity>(),
                    pagination.total,
                    pagination.page,
                    pagination.records,
                    costtime = "0",
                    ex.Message
                };
                return ToJsonResult(JsonData);
            }
        }
        #endregion
    }
}