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
    public class BudgetBLL
    {
        public void Add(BudgetEntity budget)
        {
            IBudgetService service = new BudgetService();
            service.Add(budget);
        }

        public List<BudgetEntity> GetYearBudgets(string[] deptids, string year)
        {
            IBudgetService service = new BudgetService();
            return service.GetYearBudgets(deptids, year);
        }

        public BudgetEntity GetDetail(Guid id)
        {
            IBudgetService service = new BudgetService();
            return service.GetDetail(id);
        }

        public void Modify(BudgetEntity budget)
        {
            IBudgetService service = new BudgetService();
            service.Modify(budget);
        }

        public void AddBudgets(List<BudgetEntity> budgetdata)
        {
            IBudgetService service = new BudgetService();
            service.AddBudgets(budgetdata);
        }

        public void Remove(string id)
        {
            IBudgetService service = new BudgetService();
            service.Remove(id);
        }

        public List<BudgetEntity> GetBudgetSummary(int year, string[] depts)
        {
            IBudgetService service = new BudgetService();
            return service.GetBudgetSummary(year, depts);
        }
    }
}
