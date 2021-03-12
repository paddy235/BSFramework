using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_HUMANDANGER")]
    public class HumanDanger
    {
        [Key]
        [Column("HUMANDANGERID")]
        [StringLength(36)]
        public string HumanDangerId { get; set; }
        [StringLength(500)]
        [Column("TASK")]
        public string Task { get; set; }
        [StringLength(500)]
        [Column("TASKAREA")]
        public string TaskArea { get; set; }
        [Column("DEPTID", TypeName = "text")]
        public string DeptId { get; set; }
        [Column("DEPTNAME", TypeName = "text")]
        public string DeptName { get; set; }
        [StringLength(2000)]
        [Column("TASKTYPE")]
        public string TaskType { get; set; }
        [StringLength(200)]
        [Column("DANGERLEVEL")]
        public string DangerLevel { get; set; }
        [StringLength(500)]
        [Column("OTHERMEASURE")]
        public string OtherMeasure { get; set; }
        [StringLength(50)]
        [Column("OPERATEUSER")]
        public string OperateUser { get; set; }
        [StringLength(36)]
        [Column("OPERATEUSERID")]
        public string OperateUserId { get; set; }
        [Column("OPERATETIME")]
        public DateTime OperateTime { get; set; }
        [Column("STATE")]
        public int State { get; set; }
        [InverseProperty("Danger")]
        public List<HumanDangerMeasure> Measures { get; set; }
    }

    [Table("WG_HUMANDANGERMEASURE")]
    public class HumanDangerMeasure
    {
        [Key]
        [Column("HUMANDANGERMEASUREID")]
        [StringLength(36)]
        public string HumanDangerMeasureId { get; set; }
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [StringLength(200)]
        [Column("CATEGORY")]
        public string Category { get; set; }
        [Column("MEASUREID")]
        [StringLength(36)]
        public string MeasureId { get; set; }
        [StringLength(500)]
        [Column("DANGERREASON")]
        public string DangerReason { get; set; }
        [StringLength(500)]
        [Column("MEASURECONTENT")]
        public string MeasureContent { get; set; }
        [Column("HUMANDANGERID")]
        [StringLength(36)]
        public string HumanDangerId { get; set; }
        [ForeignKey("HumanDangerId")]
        public HumanDanger Danger { get; set; }
    }

}
