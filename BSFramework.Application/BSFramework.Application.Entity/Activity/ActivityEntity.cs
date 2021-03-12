using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.QuestionManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{



    /// <summary>
    /// 活动
    /// </summary>
    [Table("WG_ACTIVITY")]
    public class ActivityEntity : BaseEntity
    {
        public ActivityEntity()
        {
            this.Files = new List<FileInfoEntity>();
            this.ActivityPersons = new List<ActivityPersonEntity>();
        }
        /// <summary>
        /// 
        /// </summary>
        [Column("ACTIVITYID")]
        public string ActivityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ACTIVITYLIMITED")]
        public string ActivityLimited { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ACTIVITYPLACE")]
        public string ActivityPlace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("SUBJECT")]
        public string Subject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CHAIRPERSONID")]
        public string ChairPersonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CHAIRPERSON")]
        public string ChairPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("RECORDPERSON")]
        public string RecordPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("LEADER")]
        public string Leader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ALERTTYPE")]
        public string AlertType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ALERTTIME")]
        public DateTime AlertTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ACTIVITYTYPE")]
        public string ActivityType { get; set; }
        /// <summary>
        /// 状态 Prepare/Ready/Study/Finish
        /// </summary>
        [Column("STATE")]
        public string State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("GROUPID")]
        public string GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string PersonId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string Persons { get; set; }

        /// <summary>
        /// 其他参与人员
        /// </summary>
        [Column("OTHERPERSONS")]
        public string OtherPersons { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("PLANSTARTTIME")]
        public DateTime PlanStartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("PLANENDTIME")]
        public DateTime PlanEndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("STARTTIME")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ENDTIME")]
        public DateTime EndTime { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<ActivityPersonEntity> ActivityPersons { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<ActivityRecordEntity> ActivityRecords { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<ActivityEvaluateEntity> Evaluates { get; set; }

        [NotMapped]
        public IList<ActivitySupplyEntity> Supplys { get; set; }

        [NotMapped]
        public IList<HistoryQuestionEntity> HistoryQuestion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string GroupName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string safetyday { get; set; }

        [NotMapped]
        public List<SubActivityEntity> SubActivities { get; set; }
        [NotMapped]
        public bool? HasSub { get; set; }
    }


    [Table("WG_SUBACTIVITY")] 
    public class SubActivityEntity
    {
        [Column("SUBJECTID")]
        public string SubjectId { get; set; }
        [Column("ACTIVITYSUBJECT")]
        public string ActivitySubject { get; set; }
        [Column("SEQ")]
        public int? Seq { get; set; }
        [Column("ACTIVITYID")]
        public string ActivityId { get; set; }
        [Column("SUBACTIVITYID")]
        public string SubActivityId { get; set; }
    }

    public class newActivity
    {
        public string DeptName { get; set; }
        public string GroupName { get; set; }
        public string FromTo { get; set; }
        public string Remark { get; set; }
    }
}
