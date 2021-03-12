using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    public class HumanDangerTrainingEntity : BaseEntity
    {
        public string TrainingId { get; set; }
        public string TrainingTask { get; set; }
        public string TrainingPlace { get; set; }
        public string No { get; set; }
        public string HumanDangerId { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUserId { get; set; }
        public bool IsDone { get; set; }
        public bool IsMarked { get; set; }
        public bool IsEvaluate { get; set; }
        public int Evaluate { get; set; }
        public string DangerLevel { get; set; }
        public string MeetingJobId { get; set; }
        public List<TaskTypeEntity> TaskTypes { get; set; }
        public List<TrainingUserEntity> TrainingUsers { get; set; }
        public List<TrainingMeasureEntity> Measures { get; set; }
        public string OtherMeasure { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime? TrainingTime { get; set; }
        public string TrainingUserId { get; set; }
        public List<ActivityEvaluateEntity> Evaluates { get; set; }
        public int EvaluateTimes { get; set; }
        public int Seq { get; set; }
        public bool IsSubmit { get; set; }
        public string AutoEvaluate { get; set; }
        public string EvaluateContent { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TicketId { get; set; }
        public string RangeTime { get; set; }
    }

    public class TrainingUserEntity
    {
        public string TrainingUserId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public int TrainingRole { get; set; }

    }

    public class TrainingMeasureEntity
    {
        public Guid TrainingMeasureId { get; set; }
        public string MeasureId { get; set; }
        public string DangerReason { get; set; }
        public string MeasureContent { get; set; }
        public string Standard { get; set; }
        /// <summary>
        /// 类型（0：多出项；1：正确项；2：缺失项；3：其他类别的）
        /// </summary>
        public int State { get; set; }
        public string CategoryId { get; set; }
        public string Category { get; set; }
    }

    public class TaskTypeEntity
    {
        public Guid TaskTypeId { get; set; }
        public string TaskTypeName { get; set; }
        /// <summary>
        /// 0 多余项
        /// 2 遗漏项
        /// 1 正确项
        /// </summary>
        public int State { get; set; }
    }

    public class HumanDangerTrainingBaseEntity
    {
        [Column("TRAININGID")]
        [StringLength(36)]
        public string TrainingId { get; set; }
        [Column("TRAININGTASK")]
        public string TrainingTask { get; set; }
        [Column("HUMANDANGERID")]
        public string HumanDangerId { get; set; }
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [Column("MEETINGJOBID")]
        public string MeetingJobId { get; set; }
        public List<HumanDangerTrainingUserEntity> TrainingUsers { get; set; }
    }

    public class HumanDangerTrainingUserEntity
    {
        [Key]
        [Column("TRAININGUSERID")]
        [StringLength(36)]
        public string TrainingUserId { get; set; }
        [Column("USERID")]
        public string UserId { get; set; }
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("TRAININGPLACE")]
        public string TrainingPlace { get; set; }
        [Column("NO")]
        public string No { get; set; }
        [Column("DANGERLEVEL")]
        public string DangerLevel { get; set; }
        [Column("OTHERMEASURE")]
        public string OtherMeasure { get; set; }
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
        /// <summary>
        /// 1:取消
        /// </summary>
        [Column("STATUS")]
        public int Status { get; set; }
        [Column("TRAININGID")]
        [StringLength(36)]
        public string TrainingId { get; set; }
        [Column("TRAININGTIME")]
        public DateTime? TrainingTime { get; set; }
        [Column("TICKETID")]
        public string TicketId { get; set; }
        public HumanDangerTrainingBaseEntity Training { get; set; }
        public List<HumanDangerTrainingMeasureEntity> TrainingMeasures { get; set; }
        public List<HumanDangerTrainingTypeEntity> TrainingTypes { get; set; }
    }

    public class HumanDangerTrainingMeasureEntity
    {
        [Column("TRAININGMEASUREID")]
        public Guid TrainingMeasureId { get; set; }
        [Column("DANGERREASON")]
        public string DangerReason { get; set; }
        [Column("MEASURECONTENT")]
        public string MeasureContent { get; set; }
        [Column("MEASUREID")]
        [StringLength(36)]
        public string MeasureId { get; set; }
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [Column("CATEGORY")]
        public string Category { get; set; }
        [Column("STATE")]
        public int State { get; set; }
        [Column("TRAININGUSERID")]
        [StringLength(36)]
        public string TrainingUserId { get; set; }
        [Column("STANDARD")]
        public string Standard { get; set; }
        public HumanDangerTrainingUserEntity Training { get; set; }
    }

    public class HumanDangerTrainingTypeEntity
    {
        [Column("TASKTYPEID")]
        public Guid TaskTypeId { get; set; }
        [Column("TYPENAME")]
        public string TypeName { get; set; }
        [Column("STATE")]
        public int State { get; set; }
        [Column("TRAININGUSERID")]
        [StringLength(36)]
        public string TrainingUserId { get; set; }
        public HumanDangerTrainingUserEntity Training { get; set; }
    }
}
