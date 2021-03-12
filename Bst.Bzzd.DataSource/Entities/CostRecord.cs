using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_COSTRECORD")]
    public class CostRecord
    {
        [Key]
        [Column("RECORDID")]
        public Guid RecordId { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [StringLength(30)]
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [StringLength(20)]
        [Column("CATEGORY")]
        public string Category { get; set; }
        [Column("BUDGETAMOUNT")]
        public decimal BudgetAmount { get; set; }
        [InverseProperty("CostRecord")]
        public List<CostItem> CostItems { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("RECORDUSERID")]
        public string RecordUserId { get; set; }
        [StringLength(30)]
        [Column("RECORDUSER")]
        public string RecordUser { get; set; }
        [Column("RECORDMONTH")]
        public DateTime RecordMonth { get; set; }
        [Column("RECORDTIME")]
        public DateTime RecordTime { get; set; }

        [Column("PROFESSIONAL")]
        public string Professional { get; set; }
    }

    [Table("WG_COSTITEM")]
    public class CostItem
    {
        [Key]
        [Column("COSTITEMID")]
        public Guid CostItemId { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [StringLength(30)]
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [Column("AMOUNT")]
        public decimal Amount { get; set; }
        [Column("COSTRECORDID")]
        public Guid CostRecordId { get; set; }
        [ForeignKey("CostRecordId")]
        public CostRecord CostRecord { get; set; }
    }
}
