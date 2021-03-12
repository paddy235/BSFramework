using BSFramework.Application.IService.BudgetAbout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.BudgetAbout;
using Bst.Bzzd.DataSource;
using Bst.Bzzd.DataSource.Entities;
using BSFramework.Application.Service.BaseManage;

namespace BSFramework.Application.Service.BudgetAbout
{
    public class CostService : ICostService
    {
        public void AddCost(CostRecordEntity record)
        {
            var entity = new CostRecord()
            {
                RecordId = record.RecordId,
                DeptId = record.RecordDeptId,
                DeptName = record.RecordDept,
                Category = record.Category,
                BudgetAmount = record.BudgetAmount,
                RecordUserId = record.RecordUserId,
                RecordUser = record.RecordUser,
                RecordMonth = record.RecordMonth,
                RecordTime = record.RecordTime,
                Professional = record.professional,
                CostItems = record.CostItems.Select(x => new CostItem()
                {
                    CostItemId = x.CostItemId,
                    DeptId = x.DeptId,
                    DeptName = x.DeptName,
                    Amount = x.Amount
                }).ToList()
            };

            var departmentservie = new DepartmentService();
            var depts = departmentservie.GetSubDepartments(entity.DeptId, null).Select(x => x.DepartmentId).ToList();
            var year = entity.RecordMonth.Year.ToString();
            var month = entity.RecordMonth.Month.ToString();
            using (var ctx = new DataContext())
            {
                var cnt = ctx.CostRecords.Where(x => x.DeptId == entity.DeptId && x.Category == entity.Category && x.RecordMonth == entity.RecordMonth).Count();
                if (cnt > 0) throw new Exception("已经存在费用记录！");

                var ddd = ctx.Budgets.Include("BudgetItems").Where(x => x.Category == entity.Category && x.Year == year).ToList();
                ddd = ddd.Where(x => depts.Any(y => y == x.DeptId)).ToList();
                var sss = ddd.SelectMany(x => x.BudgetItems).Where(x => x.Month == month);
                entity.BudgetAmount = sss.Sum(x => x.Amount);
                ctx.CostRecords.Add(entity);
                ctx.SaveChanges();
            }
        }


        public List<CostRecordEntity> GetCostRecords(int page, int pagesize, out int total, string[] deptids, string year, string month, string category, string dept)
        {
            var result = new List<CostRecordEntity>();
            using (var ctx = new DataContext())
            {
                var query = ctx.CostRecords.AsQueryable();

                if (deptids != null)
                    query = query.Where(x => deptids.Contains(x.DeptId));
                if (!string.IsNullOrEmpty(year))
                {
                    var from = new DateTime(int.Parse(year), 1, 1);
                    if (!string.IsNullOrEmpty(month))
                        from = from.AddMonths(int.Parse(month) - 1);
                    var to = from.AddMonths(1).AddMinutes(-1);
                    if (string.IsNullOrEmpty(month))
                        to = to.AddMonths(11);
                    query = query.Where(x => x.RecordMonth >= from && x.RecordMonth <= to);
                }
                if (!string.IsNullOrEmpty(category))
                    query = query.Where(x => x.Category == category);
                if (!string.IsNullOrEmpty(dept))
                    query = query.Where(x => x.DeptName.Contains(dept));

                total = query.Count();
                var data = query.OrderByDescending(x => x.RecordMonth).ThenBy(x => x.DeptName).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                foreach (var item in data)
                {
                    var record = new CostRecordEntity()
                    {
                        RecordId = item.RecordId,
                        RecordDeptId = item.DeptId,
                        RecordDept = item.DeptName,
                        RecordUserId = item.RecordUserId,
                        RecordUser = item.RecordUser,
                        Category = item.Category,
                        RecordMonth = item.RecordMonth,
                        BudgetAmount = item.BudgetAmount,
                        RecordTime = item.RecordTime
                    };
                    result.Add(record);
                }
            }
            return result;
        }

        public List<CostItemEntity> GetCostSummary(int year, string[] depts)
        {
            var from = new DateTime(year, 1, 1);
            var to = from.AddYears(1).AddMinutes(-1);

            var result = new List<CostItemEntity>();

            using (var ctx = new DataContext())
            {
                var query = from q1 in ctx.CostItems
                            join q2 in ctx.CostRecords on q1.CostRecordId equals q2.RecordId
                            where depts.Any(x => x == q1.DeptId) && q2.RecordMonth >= @from && q2.RecordMonth <= to
                            select new { q1.CostItemId, q1.Amount, q1.DeptId, q1.DeptName, q2.Category, q2.RecordMonth };

                var data = query.ToList();
                foreach (var item in data)
                {
                    result.Add(new CostItemEntity() { CostItemId = item.CostItemId, Amount = item.Amount, Category = item.Category, DeptId = item.DeptId, DeptName = item.DeptName, Month = item.RecordMonth.Month });
                }
            }

            return result;
        }

        public new List<CostRecordEntity> GetDeptRecord(int pagesize, int page, out int total, string deptid, DateTime? from, DateTime? to)
        {
            var result = new List<CostRecordEntity>();

            using (var ctx = new DataContext())
            {
                var query = from q in ctx.CostRecords
                            where q.DeptId == deptid
                            select q;

                if (from != null) query = query.Where(x => x.RecordMonth >= from);
                if (to != null) query = query.Where(x => x.RecordMonth < to);

                total = query.Count();
                var data = query.OrderByDescending(x => x.RecordMonth).ThenByDescending(x => x.RecordTime).Skip(pagesize * (page - 1)).Take(pagesize).ToList();
                foreach (var item in data)
                {
                    result.Add(new CostRecordEntity() { RecordId = item.RecordId,professional=item.Professional, BudgetAmount = item.BudgetAmount, RecordDeptId = item.DeptId, RecordDept = item.DeptName, Category = item.Category, RecordUserId = item.RecordUserId, RecordUser = item.RecordUser, RecordTime = item.RecordTime, RecordMonth = item.RecordMonth });
                }
            }
            return result;
        }

        public void Remove(string id)
        {
            if (string.IsNullOrEmpty(id)) return;

            using (var ctx = new DataContext())
            {
                var entity = ctx.CostRecords.Find(Guid.Parse(id));
                ctx.CostRecords.Remove(entity);

                ctx.SaveChanges();
            }
        }
    }
}
