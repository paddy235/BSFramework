using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Code;
using BSFramework.Busines.WorkMeeting;
using BSFrameWork.Application.AppInterface.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    /// <summary>
    /// 班组活动和教育培训统计
    /// </summary>
    [RoutePrefix("api/ActivityStatistics")]
    public class ActivityStatisticsController : BaseApiController
    {
        ActivityBLL activityBLL = new ActivityBLL();
        DepartmentBLL departmentBLL = new DepartmentBLL();

        /// <summary>
        /// 统计
        /// </summary>
        /// <param name="year">年</param>
        /// <param name="month">月</param>
        /// <param name="categories">类型</param>
        /// <returns></returns>
        [Route("")]
        public IEnumerable<ActivityStatiticsModel> Get(int year, int month, [FromUri] string[] categories)
        {
            var currentUser = OperatorProvider.Provider.Current();
            var depts = departmentBLL.GetSubDepartments(currentUser.DeptId, null);

            var from = new DateTime(year, month, 1);
            var to = from.AddMonths(1);
            var aliases = new List<string>();
            foreach (var item in categories)
            {
                aliases.AddRange(TranslateCategory(item));
            }
            var data = activityBLL.Statistics(depts.Select(x => x.DepartmentId).ToArray(), aliases.ToArray(), from, to);
            var list = data.Select(x => new ActivityStatiticsModel { Category = TranslateAlias(x.Category), Count = x.Count });
            list = list.GroupBy(x => x.Category).Select(x => new ActivityStatiticsModel { Category = x.Key, Count = x.Sum(y => y.Count) }).Where(x => categories.Contains(x.Category)).ToList();
            return list;
        }

        private string TranslateAlias(string alias)
        {
            var category = alias;
            switch (alias)
            {
                case "2":
                    alias = "TQ";
                    break;
                case "5":
                    alias = "TQ";
                    break;
                case "3":
                    alias = "AA";
                    break;
                case "6":
                    alias = "AA";
                    break;
                case "1":
                    alias = "TC";
                    break;
                case "7":
                    alias = "EX";
                    break;
                case "8":
                    alias = "EX";
                    break;
                case "4":
                    alias = "AT";
                    break;
                case "安全学习日":
                    alias = "SS";
                    break;
                case "安全日活动":
                    alias = "SD";
                    break;
                case "政治学习":
                    alias = "PO";
                    break;
                case "民主管理会":
                    alias = "DE";
                    break;
                case "班务会":
                    alias = "CW";
                    break;
                case "上级精神宣贯":
                    alias = "SI";
                    break;
                default:
                    alias = "其他";
                    break;
            }
            return alias;
        }

        private string[] TranslateCategory(string category)
        {
            string[] alias;
            switch (category)
            {
                case "TQ":
                    alias = new string[] { "2", "5" };
                    break;
                case "AA":
                    alias = new string[] { "3", "6" };
                    break;
                case "TC":
                    alias = new string[] { "1" };
                    break;
                case "EX":
                    alias = new string[] { "7", "8" };
                    break;
                case "AT":
                    alias = new string[] { "4" };
                    break;
                case "SS":
                    alias = new string[] { "安全学习日" };
                    break;
                case "SD":
                    alias = new string[] { "安全日活动" };
                    break;
                case "PO":
                    alias = new string[] { "政治学习" };
                    break;
                case "DE":
                    alias = new string[] { "民主管理会" };
                    break;
                case "CW":
                    alias = new string[] { "班务会" };
                    break;
                case "SI":
                    alias = new string[] { "上级精神宣贯" };
                    break;
                default:
                    alias = new string[] { category };
                    break;
            }
            return alias;
        }
    }
}