using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_BUDGET")]
    public class Budget
    {
        [Key]
        [Column("BUDGETID")]
        public Guid BudgetId { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [StringLength(30)]
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [StringLength(10)]
        [Column("YEAR")]
        public string Year { get; set; }
        [StringLength(30)]
        [Column("CATEGORY")]
        public string Category { get; set; }
        [InverseProperty("Budget")]
        public List<BudgetDetail> BudgetItems { get; set; }
    }

    [Table("WG_BUDGETDETAIL")]
    public class BudgetDetail
    {
        [Key]
        [Column("DETAILID")]
        public Guid DetailId { get; set; }
        [StringLength(2)]
        [Column("MONTH")]
        public string Month { get; set; }
        [Column("AMOUNT")]
        public decimal Amount { get; set; }
        [Column("BUDGETID")]
        public Guid BudgetId { get; set; }
        [ForeignKey("BudgetId")]
        public Budget Budget { get; set; }
    }
}
