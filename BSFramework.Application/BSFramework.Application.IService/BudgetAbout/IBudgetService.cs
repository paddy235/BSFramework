using BSFramework.Application.Entity.BudgetAbout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.BudgetAbout
{
    public interface IBudgetService
    {
        void Add(BudgetEntity budget);
        List<BudgetEntity> GetYearBudgets(string[] deptids, string year);
        BudgetEntity GetDetail(Guid id);
        void Modify(BudgetEntity budget);
        void AddBudgets(List<BudgetEntity> budgetdata);
        void Remove(string id);
        List<BudgetEntity> GetBudgetSummary(int year, string[] depts);
    }
}
