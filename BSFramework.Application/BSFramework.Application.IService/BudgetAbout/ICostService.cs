using BSFramework.Application.Entity.BudgetAbout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.BudgetAbout
{
    public interface ICostService
    {
        void AddCost(CostRecordEntity record);
        List<CostRecordEntity> GetCostRecords(int page, int pagesize, out int total, string[] deptids, string year, string month, string category, string dept);
        void Remove(string id);
        List<CostItemEntity> GetCostSummary(int year, string[] depts);
        new List<CostRecordEntity> GetDeptRecord(int pagesize, int page, out int total, string deptid, DateTime? from, DateTime? to);
    }
}
