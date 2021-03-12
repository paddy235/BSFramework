using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.BudgetAbout
{
    public class CostRecordEntity
    {
        public Guid RecordId { get; set; }
        public string RecordUserId { get; set; }
        public string RecordUser { get; set; }
        public DateTime RecordMonth { get; set; }
        public string Category { get; set; }
        public string RecordDeptId { get; set; }
        public string RecordDept { get; set; }
        public decimal BudgetAmount { get; set; }
        public DateTime RecordTime { get; set; }
        public List<CostItemEntity> CostItems { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string professional { get; set; }
    }

    public class CostItemEntity
    {
        public Guid CostItemId { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public int Month { get; set; }
    }
}
