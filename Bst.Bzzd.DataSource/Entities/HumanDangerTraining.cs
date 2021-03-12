using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_HUMANDANGERTRAINING")]
    public class HumanDangerTraining
    {
        [Key]
        [Column("TRAININGID")]
        [StringLength(36)]
        public string TrainingId { get; set; }
        [StringLength(500)]
        [Column("TRAININGTASK")]
        public string TrainingTask { get; set; }
        [Index(IsClustered = true)]
        [Column("HUMANDANGERID")]
        [StringLength(36)]
        public string HumanDangerId { get; set; }
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
        [StringLength(36)]
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [StringLength(50)]
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [StringLength(36)]
        [Column("MEETINGJOBID")]
        public string MeetingJobId { get; set; }
        [InverseProperty("Training")]
        public List<HumanDangerTrainingUser> TrainingUsers { get; set; }
    }

    [Table("WG_TRAININGUSER")]
    public class HumanDangerTrainingUser
    {
        [Key]
        [Column("TRAININGUSERID")]
        [StringLength(36)]
        public string TrainingUserId { get; set; }
        [StringLength(36)]
        [Index(IsClustered = true)]
        [Column("USERID")]
        public string UserId { get; set; }
        [StringLength(100)]
        [Column("USERNAME")]
        public string UserName { get; set; }
        [StringLength(500)]
        [Column("TRAININGPLACE")]
        public string TrainingPlace { get; set; }
        [StringLength(2000)]
        [Column("NO")]
        public string No { get; set; }
        [StringLength(2000)]
        [Column("TICKETID")]
        public string TicketId { get; set; }
        [StringLength(20)]
        [Column("DANGERLEVEL")]
        public string DangerLevel { get; set; }
        [StringLength(500)]
        [Column("OTHERMEASURE")]
        public string OtherMeasure { get; set; }
        /// <summary>
        /// 作业人类型（0：作业人；1：负责人）
        /// </summary>
        [Column("TRAININGROLE")]
        public int TrainingRole { get; set; }
        [Column("ISDONE")]
        public bool IsDone { get; set; }
        [Column("ISMARKED")]
        public bool IsMarked { get; set; }
        [Column("ISEVALUATED")]
        public bool IsEvaluated { get; set; }
        [Column("EVALUATETIMES")]
        public int EvaluateTimes { get; set; }
        [Column("STATUS")]
        public int Status { get; set; }
        [Column("TRAININGTIME")]
        public DateTime? TrainingTime { get; set; }
        [Column("TRAININGID")]
        [StringLength(36)]
        public string TrainingId { get; set; }
        [ForeignKey("TrainingId")]
        public HumanDangerTraining Training { get; set; }
        [InverseProperty("Training")]
        public List<TrainingType> TrainingTypes { get; set; }
        [InverseProperty("Training")]
        public List<TrainingMeasure> TrainingMeasures { get; set; }
    }

    [Table("WG_TRAININGMEASURE")]
    public class TrainingMeasure
    {
        [Key]
        [Column("TRAININGMEASUREID")]
        public Guid TrainingMeasureId { get; set; }
        [StringLength(500)]
        [Column("DANGERREASON")]
        public string DangerReason { get; set; }
        [StringLength(500)]
        [Column("MEASURECONTENT")]
        public string MeasureContent { get; set; }
        [StringLength(500)]
        [Column("STANDARD")]
        public string Standard { get; set; }
        [Column("MEASUREID")]
        [StringLength(36)]
        public string MeasureId { get; set; }
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [StringLength(200)]
        [Column("CATEGORY")]
        public string Category { get; set; }
        /// <summary>
        /// 类型（0：多出项；1：正确项；2：缺失项；3：其他类别的）
        /// </summary>
        [Column("STATE")]
        public int State { get; set; }
        [Column("TRAININGUSERID")]
        [StringLength(36)]
        public string TrainingUserId { get; set; }
        [ForeignKey("TrainingUserId")]
        public HumanDangerTrainingUser Training { get; set; }
    }

    [Table("WG_TRAININGTYPE")]
    public class TrainingType
    {
        [Key]
        [Column("TASKTYPEID")]
        public Guid TaskTypeId { get; set; }
        [StringLength(200)]
        [Column("TYPENAME")]
        public string TypeName { get; set; }
        [Column("STATE")]
        public int State { get; set; }
        [Column("TRAININGUSERID")]
        [StringLength(36)]
        public string TrainingUserId { get; set; }
        [ForeignKey("TrainingUserId")]
        public HumanDangerTrainingUser Training { get; set; }
    }
}
