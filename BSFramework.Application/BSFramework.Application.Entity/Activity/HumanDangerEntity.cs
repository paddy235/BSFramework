using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    [Table("WG_HUMANDANGER")]
    public class HumanDangerEntity
    {
        [Column("HUMANDANGERID")]
        [StringLength(36)]
        public string HumanDangerId { get; set; }
        [Column("DANGERLEVEL")]
        public string DangerLevel { get; set; }
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [Column("OPERATETIME")]
        public DateTime OperateTime { get; set; }
        [Column("TASK")]
        public string Task { get; set; }
        [Column("TASKAREA")]
        public string TaskArea { get; set; }
        [Column("TASKTYPE")]
        public string TaskType { get; set; }
        [Column("OPERATEUSER")]
        public string OperateUser { get; set; }
        [Column("OPERATEUSERID")]
        public string OperateUserId { get; set; }
        [Column("OTHERMEASURE")]
        public string OtherMeasure { get; set; }
        [Column("STATE")]
        public int State { get; set; }
        [NotMapped]
        public string ApproveUser { get; set; }
        [NotMapped]
        public DateTime? ApproveTime { get; set; }
        [NotMapped]
        public string StateDescription { get; set; }
        [NotMapped]
        public List<HumanDangerMeasureEntity> Measures { get; set; }

        [NotMapped]
        public List<ApproveRecordEntity> ApproveRecords { get; set; }
    }

    [Table("WG_HUMANDANGERMEASURE")]
    public class HumanDangerMeasureEntity
    {
        [Column("HUMANDANGERMEASUREID")]
        [StringLength(36)]
        public string HumanDangerMeasureId { get; set; }
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [Column("CATEGORY")]
        public string Category { get; set; }
        [Column("MEASUREID")]
        [StringLength(36)]
        public string MeasureId { get; set; }
        [Column("DANGERREASON")]
        public string DangerReason { get; set; }
        [Column("MEASURECONTENT")]
        public string MeasureContent { get; set; }
        [Column("HUMANDANGERID")]
        [StringLength(36)]
        public string HumanDangerId { get; set; }
    }

    [Table("WG_APPROVERECORD")]
    public class ApproveRecordEntity
    {
        [Column("APPROVERECORDID")]
        public Guid ApproveRecordId { get; set; }
        [Column("APPROVEUSERID")]
        public string ApproveUserId { get; set; }
        [Column("APPROVEUSER")]
        public string ApproveUser { get; set; }
        [Column("APPROVEDEPTID")]
        public string ApproveDeptId { get; set; }
        [Column("APPROVEDEPTNAME")]
        public string ApproveDeptName { get; set; }
        [Column("APPROVETIME")]
        public DateTime ApproveTime { get; set; }
        [Column("RECORDID")]
        public string RecordId { get; set; }
    }
}
