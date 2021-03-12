using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.BudgetAbout;
using BSFramework.Application.IService.BudgetAbout;
using BSFramework.Application.Service.BudgetAbout;

namespace BSFramework.Application.Busines.BudgetAbout
{
    public class CostBLL
    {
        public void AddCost(CostRecordEntity record)
        {
            ICostService service = new CostService();
            service.AddCost(record);
        }

        public List<CostRecordEntity> GetCostRecords(int page, int pagesize, out int total, string[] deptids, string year, string month, string category, string dept)
        {
            ICostService service = new CostService();
            return service.GetCostRecords(page, pagesize, out total, deptids, year, month, category, dept);
        }

        public void Remove(string id)
        {
            ICostService service = new CostService();
            service.Remove(id);
        }

        public List<CostItemEntity> GetCostSummary(int year, string[] depts)
        {
            ICostService service = new CostService();
            return service.GetCostSummary(year, depts);
        }

        public List<CostRecordEntity> GetDeptRecord(int pagesize, int page, out int total, string deptid, DateTime? from, DateTime? to)
        {
            ICostService service = new CostService();
            return service.GetDeptRecord(pagesize, page, out total, deptid, from, to);
        }
    }
}
