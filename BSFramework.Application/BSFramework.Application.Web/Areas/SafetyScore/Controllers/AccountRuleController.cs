using Aspose.Cells;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.SafetyScore;
using BSFramework.Application.Code;
using BSFramework.Application.Entity.SafetyScore;
using BSFramework.Util;
using BSFramework.Util.Log;
using BSFramework.Util.WebControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BSFramework.Application.Web.Areas.SafetyScore.Controllers
{
    /// <summary>
    /// 积分规则
    /// </summary>
    public class AccountRuleController : MvcControllerBase
    {
        private readonly AccountRuleBLL _bll;
        public AccountRuleController()
        {
            _bll = new AccountRuleBLL();
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
            return View();
        }
        /// <summary>
        /// 表单页面 
        /// </summary>
        /// <returns></returns>
        public ActionResult Form()
        {
            return View();
        }

        public ActionResult ImportPage()
        {
            return View();
        }
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
                IEnumerable<AccountRuleEntity> data = _bll.GetPagedList(pagination, queryJson);
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
                    rows = new List<AccountRuleEntity>(),
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
                AccountRuleEntity entity = _bll.GetEntity(keyValue);
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
        public ActionResult SaveForm(string keyValue, AccountRuleEntity entity)
        {
            Operator user = OperatorProvider.Provider.Current();
            WriteLog.AddLog($"用户：{user.UserName}  id：{user.UserId}开始编辑积分配置{keyValue} \r\n {JsonConvert.SerializeObject(entity)}", "SafetyScore");
            try
            {
                if (_bll.SaveForm(keyValue, entity))
                    return Success("操作成功");
                else
                    return Error("操作失败");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"错误 \r\n用户：{user.UserName}  id：{user.UserId}编辑积分配置{keyValue}失败 \r\n 错误信息：{JsonConvert.SerializeObject(ex)}", "SafetyScore");
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
            WriteLog.AddLog($"用户：{user.UserName}  id：{user.UserId}开始删除积分配置{keyValue}", "SafetyScore");
            try
            {
                _bll.Remove(keyValue);
                return Success("删除成功。");
            }
            catch (Exception ex)
            {
                WriteLog.AddLog($"错误 \r\n用户：{user.UserName}  id：{user.UserId}删除积分配置{keyValue}失败 \r\n 错误信息：{JsonConvert.SerializeObject(ex)}", "SafetyScore");
                return Error("删除失败：" + ex.Message);
            }
        }

        [HttpPost]
        [AjaxOnly]
        [HandlerMonitor(8, "导入安全卫士积分标准")]
        public ActionResult Import()
        {
            Operator user = OperatorProvider.Provider.Current();
            try
            {
                if (this.Request.Files.Count == 0) throw new Exception("请上传文件");
                if (!this.Request.Files[0].FileName.EndsWith(".xlsx")) return Json(new { success = false, message = "请上传excel文件！" });

                var book = new Workbook(this.Request.Files[0].InputStream);
                var sheet = book.Worksheets[0];

                if (sheet.Cells[1, 0].StringValue != "标准*" || sheet.Cells[1, 1].StringValue != "分值*" || sheet.Cells[1, 2].StringValue != "顺序号" || sheet.Cells[1, 3].StringValue != "备注")
                {
                    return Json(new { success = false, message = "请使用正确的模板导入！" });
                }


                List<AccountRuleEntity> importList = new List<AccountRuleEntity>();
                for (int i = 2; i <= sheet.Cells.MaxDataRow; i++)
                {
                    if (string.IsNullOrEmpty(sheet.Cells[i, 0].StringValue)) { throw new Exception($"行号{i + 1}的”标准“不能为空"); }
                    if (sheet.Cells[i, 0].StringValue.Length > 512) { throw new Exception($"行号{i + 1}的”标准“长度不能超过512"); }
                    if (string.IsNullOrEmpty(sheet.Cells[i, 1].StringValue)) { throw new Exception($"行号{i + 1}的”分值“不能为空"); }

                    decimal score = 0;
                    int sort = 0;
                    if (decimal.TryParse(sheet.Cells[i, 1].StringValue, out score))
                    {
                        Math.Round(score, 1);
                    }
                    else
                    {
                        throw new Exception($"行号{i + 1}的”分值“必须为小数");
                    }
                    if (score < 0) throw new Exception($"行号{i + 1}的”分值“不能为负数");
                    if (!string.IsNullOrEmpty(sheet.Cells[i, 2].StringValue))
                    {
                        if (int.TryParse(sheet.Cells[i, 2].StringValue, out sort)) { }
                    }

                    AccountRuleEntity import = new AccountRuleEntity()
                    {

                        Standard = sheet.Cells[i, 0].StringValue,
                        Score = score,
                        ScoreType = Enum.GetName(typeof(ScoreType), ScoreType.手动),
                        Sort = sort,
                        Remark = sheet.Cells[i, 3].StringValue,
                        IsOpen = 1,
                    };
                    import.Create();
                    importList.Add(import);
                }
                if (importList.Count > 0) _bll.Insert(importList);
                return Json(new { success = true, message = "导入成功" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion
    }
}