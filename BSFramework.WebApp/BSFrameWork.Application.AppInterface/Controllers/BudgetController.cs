using BSFramework.Application.Busines.Activity;
using BSFramework.Application.Busines.BaseManage;
using BSFramework.Application.Busines.BudgetAbout;
using BSFramework.Application.Busines.PublicInfoManage;
using BSFramework.Application.Busines.SystemManage;
using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.Entity.BudgetAbout;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Busines.WorkMeeting;
using BSFramework.Util;
using BSFrameWork.Application.AppInterface.Models;
using Bst.Fx.Uploading;
using Bst.ServiceContract.MessageQueue;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ThoughtWorks.QRCode.Codec;

namespace BSFrameWork.Application.AppInterface.Controllers
{
    public class BudgetController : ApiController
    {
        [HttpPost]
        public ListBucket<CostSummaryModel> GetCostSummray(CostArgModel model)
        {
            var deptbll = new DepartmentBLL();
            var teams = deptbll.GetSubDepartments(model.DeptId, "班组");

            var databll = new DataItemDetailBLL();
            var dataitems = databll.GetDataItems("费用类型");

            var budgetbll = new BudgetBLL();
            var budget = budgetbll.GetBudgetSummary(model.Year, teams.Select(x => x.DepartmentId).ToArray());
            var costbll = new CostBLL();
            var cost = costbll.GetCostSummary(model.Year, teams.Select(x => x.DepartmentId).ToArray());
            var result = new List<CostSummaryModel>();
            result.AddRange(dataitems.GroupJoin(budget, x => x.ItemValue, y => y.Category, (x, y) => new CostSummaryModel { Category = x.ItemValue + "预算", Data = new decimal[] { y.Sum(z => z.Budget1), y.Sum(z => z.Budget2), y.Sum(z => z.Budget3), y.Sum(z => z.Budget4), y.Sum(z => z.Budget5), y.Sum(z => z.Budget6), y.Sum(z => z.Budget7), y.Sum(z => z.Budget8), y.Sum(z => z.Budget9), y.Sum(z => z.Budget10), y.Sum(z => z.Budget11), y.Sum(z => z.Budget12), } }));
            result.AddRange(dataitems.GroupJoin(cost, x => x.ItemValue, y => y.Category, (x, y) => new CostSummaryModel { Category = x.ItemValue + "支出", Data = new decimal[] { y.Where(z => z.Month == 1).Sum(z => z.Amount), y.Where(z => z.Month == 2).Sum(z => z.Amount), y.Where(z => z.Month == 3).Sum(z => z.Amount), y.Where(z => z.Month == 4).Sum(z => z.Amount), y.Where(z => z.Month == 5).Sum(z => z.Amount), y.Where(z => z.Month == 6).Sum(z => z.Amount), y.Where(z => z.Month == 7).Sum(z => z.Amount), y.Where(z => z.Month == 8).Sum(z => z.Amount), y.Where(z => z.Month == 9).Sum(z => z.Amount), y.Where(z => z.Month == 10).Sum(z => z.Amount), y.Where(z => z.Month == 11).Sum(z => z.Amount), y.Where(z => z.Month == 12).Sum(z => z.Amount), } }));

            result = result.OrderBy(x => x.Category).ToList();
            return new ListBucket<CostSummaryModel>() { code = 0, Data = result, Total = result.Count };
        }

        [HttpPost]
        public ListBucket<CostRecordEntity> GetCostRecord(ParamBucket<CostRecordModel> arg)
        {
            var userbll = new UserBLL();
            var user = userbll.GetEntity(arg.UserId);
            var dept = new DepartmentBLL().GetEntity(user.DepartmentId);

            var from = string.IsNullOrEmpty(arg.Data.From) ? null : (DateTime?)DateTime.Parse(arg.Data.From);
            var to = string.IsNullOrEmpty(arg.Data.To) ? null : (DateTime?)DateTime.Parse(arg.Data.To);

            var total = 0;
            var costbll = new CostBLL();
            var data = costbll.GetDeptRecord(arg.PageSize, arg.PageIndex, out total, dept.ParentId, from, to);
            var filebll = new FileInfoBLL();
            var urlpath = Config.GetValue("AppUrl");
            foreach (var item in data)
            {
                var files = filebll.GetFileList(item.RecordId.ToString());
                if (files.Count > 0)
                {
                    item.FilePath = urlpath + files[0].FilePath.Replace("~/", string.Empty);
                    item.FileName = files[0].FileName;
                }
            }
            return new ListBucket<CostRecordEntity>() { code = 0, Data = data, Total = total };
        }
    }
}
