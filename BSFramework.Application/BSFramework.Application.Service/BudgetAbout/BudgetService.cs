using BSFramework.Application.IService.BudgetAbout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.BudgetAbout;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;

namespace BSFramework.Application.Service.BudgetAbout
{
    public class BudgetService : IBudgetService
    {
        public void Add(BudgetEntity entity)
        {
            var budget = new Budget()
            {
                BudgetId = Guid.NewGuid(),
                Category = entity.Category,
                DeptId = entity.DeptId,
                DeptName = entity.DeptName,
                Year = entity.Year,
                BudgetItems = new List<BudgetDetail>()
            };

            this.ToBudgetItems(entity, budget);

            using (var ctx = new DataContext())
            {
                var cnt = ctx.Budgets.Where(x => x.Year == budget.Year && x.Category == budget.Category && x.DeptId == entity.DeptId).Count();
                if (cnt > 0) throw new Exception("已经存在费用预算");

                ctx.Budgets.Add(budget);
                ctx.SaveChanges();
            }
        }

        public void AddBudgets(List<BudgetEntity> budgetdata)
        {
            using (var ctx = new DataContext())
            {
                foreach (var item in budgetdata)
                {
                    if (ctx.Budgets.Count(x => x.DeptId == item.DeptId && x.Year == item.Year && x.Category == item.Category) > 0) throw new Exception(string.Format("{0}已经存在预算！", item.DeptName));
                }

                foreach (var item in budgetdata)
                {
                    var entity = new Budget()
                    {
                        BudgetId = item.BudgetId,
                        DeptId = item.DeptId,
                        DeptName = item.DeptName,
                        Year = item.Year,
                        Category = item.Category,
                        BudgetItems = new List<BudgetDetail>()
                    };
                    this.ToBudgetItems(item, entity);

                    ctx.Budgets.Add(entity);
                }

                ctx.SaveChanges();
            }
        }

        public List<BudgetEntity> GetBudgetSummary(int year, string[] depts)
        {
            var str_year = year.ToString();
            var result = new List<BudgetEntity>();
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.Budgets.Include("BudgetItems")
                            where depts.Contains(q.DeptId) && q.Year == str_year
                            select q;
                var data = query.ToList();
                foreach (var item in data)
                {
                    var budget = new BudgetEntity()
                    {
                        BudgetId = item.BudgetId,
                        Category = item.Category,
                        DeptId = item.DeptId,
                        DeptName = item.DeptName,
                        Year = item.Year
                    };

                    this.ToBudget(item, budget);
                    result.Add(budget);
                }
            }
            return result;
        }

        public BudgetEntity GetDetail(Guid id)
        {
            var data = default(Budget);
            using (var ctx = new DataContext())
            {
                data = (from q in ctx.Budgets.Include("BudgetItems")
                        where q.BudgetId == id
                        select q).FirstOrDefault();
            }
            if (data == null) return null;

            var result = new BudgetEntity()
            {
                BudgetId = data.BudgetId,
                DeptId = data.DeptId,
                DeptName = data.DeptName,
                Year = data.Year,
                Category = data.Category
            };
            this.ToBudget(data, result);

            return result;
        }

        public List<BudgetEntity> GetYearBudgets(string[] deptids, string year)
        {
            if (deptids == null) deptids = new string[] { };
            var data = default(List<Budget>);
            using (var ctx = new DataContext())
            {
                var query = from q in ctx.Budgets.Include("BudgetItems")
                            where deptids.Any(x => x == q.DeptId) && q.Year == year
                            orderby q.Year descending, q.DeptName, q.Category
                            select q;

                data = query.ToList();
            }

            var result = new List<BudgetEntity>();
            foreach (var item in data)
            {
                var budget = new BudgetEntity()
                {
                    BudgetId = item.BudgetId,
                    Year = item.Year,
                    DeptId = item.DeptId,
                    DeptName = item.DeptName,
                    Category = item.Category
                };

                this.ToBudget(item, budget);
                result.Add(budget);
            }

            return result;
        }

        public void Modify(BudgetEntity budget)
        {
            using (var ctx = new DataContext())
            {
                var entity = ctx.Budgets.Include("BudgetItems").FirstOrDefault(x => x.BudgetId == budget.BudgetId);
                if (entity != null)
                {
                    var item1 = entity.BudgetItems.Find(x => x.Month == "1");
                    if (item1 != null) item1.Amount = budget.Budget1;   ctx.Entry(item1).State = System.Data.Entity.EntityState.Modified; 
                    var item2 = entity.BudgetItems.Find(x => x.Month == "2");
                    if (item2 != null) item2.Amount = budget.Budget2; ctx.Entry(item2).State = System.Data.Entity.EntityState.Modified;
                    var item3 = entity.BudgetItems.Find(x => x.Month == "3");
                    if (item3 != null) item3.Amount = budget.Budget3; ctx.Entry(item3).State = System.Data.Entity.EntityState.Modified;
                    var item4 = entity.BudgetItems.Find(x => x.Month == "4");
                    if (item4 != null) item4.Amount = budget.Budget4; ctx.Entry(item4).State = System.Data.Entity.EntityState.Modified;
                    var item5 = entity.BudgetItems.Find(x => x.Month == "5");
                    if (item5 != null) item5.Amount = budget.Budget5; ctx.Entry(item5).State = System.Data.Entity.EntityState.Modified;
                    var item6 = entity.BudgetItems.Find(x => x.Month == "6");
                    if (item6 != null) item6.Amount = budget.Budget6; ctx.Entry(item6).State = System.Data.Entity.EntityState.Modified;
                    var item7 = entity.BudgetItems.Find(x => x.Month == "7");
                    if (item7 != null) item7.Amount = budget.Budget7; ctx.Entry(item7).State = System.Data.Entity.EntityState.Modified;
                    var item8 = entity.BudgetItems.Find(x => x.Month == "8");
                    if (item8 != null) item8.Amount = budget.Budget8; ctx.Entry(item8).State = System.Data.Entity.EntityState.Modified;
                    var item9 = entity.BudgetItems.Find(x => x.Month == "9");
                    if (item9 != null) item9.Amount = budget.Budget9; ctx.Entry(item9).State = System.Data.Entity.EntityState.Modified;
                    var item10 = entity.BudgetItems.Find(x => x.Month == "10");
                    if (item10 != null) item10.Amount = budget.Budget10; ctx.Entry(item10).State = System.Data.Entity.EntityState.Modified;
                    var item11 = entity.BudgetItems.Find(x => x.Month == "11");
                    if (item11 != null) item11.Amount = budget.Budget11; ctx.Entry(item11).State = System.Data.Entity.EntityState.Modified;
                    var item12 = entity.BudgetItems.Find(x => x.Month == "12");
                    if (item12 != null) item12.Amount = budget.Budget12; ctx.Entry(item12).State = System.Data.Entity.EntityState.Modified;

                    ctx.SaveChanges();
                }
            }
        }

        public void Remove(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            using (var ctx = new DataContext())
            {
                var entity = ctx.Budgets.Find(Guid.Parse(id));
                ctx.Budgets.Remove(entity);

                ctx.SaveChanges();
            }
        }

        private void ToBudget(Budget entity, BudgetEntity budget)
        {
            var item1 = entity.BudgetItems.Find(x => x.Month == "1");
            if (item1 != null) budget.Budget1 = item1.Amount;
            var item2 = entity.BudgetItems.Find(x => x.Month == "2");
            if (item2 != null) budget.Budget2 = item2.Amount;
            var item3 = entity.BudgetItems.Find(x => x.Month == "3");
            if (item3 != null) budget.Budget3 = item3.Amount;
            var item4 = entity.BudgetItems.Find(x => x.Month == "4");
            if (item4 != null) budget.Budget4 = item4.Amount;
            var item5 = entity.BudgetItems.Find(x => x.Month == "5");
            if (item5 != null) budget.Budget5 = item5.Amount;
            var item6 = entity.BudgetItems.Find(x => x.Month == "6");
            if (item6 != null) budget.Budget6 = item6.Amount;
            var item7 = entity.BudgetItems.Find(x => x.Month == "7");
            if (item7 != null) budget.Budget7 = item7.Amount;
            var item8 = entity.BudgetItems.Find(x => x.Month == "8");
            if (item8 != null) budget.Budget8 = item8.Amount;
            var item9 = entity.BudgetItems.Find(x => x.Month == "9");
            if (item9 != null) budget.Budget9 = item9.Amount;
            var item10 = entity.BudgetItems.Find(x => x.Month == "10");
            if (item10 != null) budget.Budget10 = item10.Amount;
            var item11 = entity.BudgetItems.Find(x => x.Month == "11");
            if (item11 != null) budget.Budget11 = item11.Amount;
            var item12 = entity.BudgetItems.Find(x => x.Month == "12");
            if (item12 != null) budget.Budget12 = item12.Amount;

            budget.Total = budget.Budget1 + budget.Budget2 + budget.Budget3 + budget.Budget4 + budget.Budget5 + budget.Budget6 + budget.Budget7 + budget.Budget8 + budget.Budget9 + budget.Budget10 + budget.Budget11 + budget.Budget12;
        }

        private void ToBudgetItems(BudgetEntity budget, Budget entity)
        {
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget1, Month = "1" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget2, Month = "2" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget3, Month = "3" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget4, Month = "4" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget5, Month = "5" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget6, Month = "6" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget7, Month = "7" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget8, Month = "8" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget9, Month = "9" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget10, Month = "10" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget11, Month = "11" });
            entity.BudgetItems.Add(new BudgetDetail() { DetailId = Guid.NewGuid(), Amount = budget.Budget12, Month = "12" });
        }
    }
}
