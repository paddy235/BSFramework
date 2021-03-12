using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.QuestionManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{



    /// <summary>
    /// 活动
    /// </summary>
    [Table("wg_edactivity")]
    public class EdActivityEntity : BaseEntity
    {
        public EdActivityEntity()
        {
            this.Files = new List<FileInfoEntity>();
            this.ActivityPersons = new List<EdActivityPersonEntity>();
        }
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityId")]
        public string ActivityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityLimited")]
        public string ActivityLimited { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityPlace")]
        public string ActivityPlace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Subject")]
        public string Subject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ChairPerson")]
        public string ChairPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("RecordPerson")]
        public string RecordPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Leader")]
        public string Leader { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("AlertType")]
        public string AlertType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("AlertTime")]
        public DateTime AlertTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityType")]
        public string ActivityType { get; set; }
        /// <summary>
        /// 状态 Prepare/Ready/Study/Finish
        /// </summary>
        [Column("state")]
        public string State { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("GroupId")]
        public string GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Remark")]
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
        /// 
        /// </summary>
        [Column("PlanStartTime")]
        public DateTime PlanStartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("PlanEndTime")]
        public DateTime PlanEndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("StartTime")]
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("EndTime")]
        public DateTime EndTime { get; set; }
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<EdActivityPersonEntity> ActivityPersons { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<EdActivityRecordEntity> ActivityRecords { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public IList<HistoryQuestionEntity> HistoryQuestion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<ActivityEvaluateEntity> Evaluates { get; set; }

        [NotMapped]
        public IList<EdActivitySupplyEntity> Supplys { get; set; }
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

    }

  
}
