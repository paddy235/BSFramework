using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.Activity;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// �������
    /// </summary>
    [Table("WG_MEETINGJOB")]
    public class MeetingJobEntity : BaseEntity
    {
        #region ʵ���Ա
        /// <summary>
        /// 
        /// </summary>
        [Column("JOBID")]
        public string JobId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("PLANID")]
        public string PlanId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("JOB")]
        public string Job { get; set; }
        /// <summary>
        [NotMapped]
        public string JobUsers { get; set; }
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
        /// <summary>
        /// 
        /// </summary>
        [Column("DANGEROUS")]
        public string Dangerous { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MEASURE")]
        public string Measure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ISFINISHED")]
        public string IsFinished { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("GROUPID")]
        public string GroupId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("SCORE")]
        public string Score { get; set; }

        [Column("TEMPLATEID")]
        public string TemplateId { get; set; }
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        [Column("NEEDTRAIN")]
        public bool NeedTrain { get; set; }
        [NotMapped]
        public bool? TrainingDone { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("JOBTYPE")]
        public string JobType { get; set; }
        [Column("JOBCATEGORY")]
        public string JobCategory { get; set; }
        [Column("JOBPROJECT1")]
        public string JobProject1 { get; set; }
        [Column("JOBDEPT")]
        public string JobDept { get; set; }
        [Column("JOBPROJECT2")]
        public string JobProject2 { get; set; }
        [Column("JOBADDR")]
        public string JobAddr { get; set; }
        [Column("JOBNO")]
        public string JobNo { get; set; }
        [Column("RECID")]
        public string RecId { get; set; }
        [Column("RESULT")]
        public string Result { get; set; }
        [Column("TIMELENGTH")]
        public string TimeLength { get; set; }
        [Column("IMAGELIST")]
        public string ImageList { get; set; }
        [NotMapped]
        public string CreateUserId { get; set; }
        [Column("FAULTID")]
        public decimal? FaultId { get; set; }
        [Column("TICKETID")]
        public string TicketId { get; set; }
        [Column("TICKETCODE")]
        public string TicketCode { get; set; }
        [Column("USERID")]
        public string UserId { get; set; }
        /// <summary>
        /// ����״̬ 1������    0δ����
        /// </summary>
        [Column("EVALUATESTATE")]
        public int EvaluateState { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        [Column("TASKTYPE")]
        public string TaskType { get; set; }

        /// <summary>
        /// ���յȼ�
        /// </summary>
        [Column("RISKLEVEL")]
        public string RiskLevel { get; set; }


        //[NotMapped]
        //public IList<JobUserEntity> Persons { get; set; }

        ///// <summary>
        ///// �ڱ�����鹤����Աʱ�������ӵĹ�����Ա
        ///// </summary>
        //[NotMapped]
        //public IList<JobUserEntity> NewPersons { get; set; }

        ///// <summary>
        ///// �ڱ�����鹤����Աʱ��ɾ���Ĺ�����Ա
        ///// </summary>
        //[NotMapped]
        //public IList<JobUserEntity> DeletePersons { get; set; }

        ///// <summary>
        ///// �ڱ�����鹤����Աʱ�������ɫʱʹ��
        ///// </summary>
        //[NotMapped]
        //public IList<JobUserEntity> UpdatePersons { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> FileList1 { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> FileList2 { get; set; }
        [NotMapped]
        public MeetingAndJobEntity Relation { get; set; }
        [NotMapped]
        public List<JobDangerousEntity> DangerousList { get; set; }
        [NotMapped]
        public DangerEntity Training { get; set; }
        [NotMapped]
        public string Status { get; set; }
        [NotMapped]
        public int? piccount { get; set; }
        [NotMapped]
        public int? audiocount { get; set; }
        [NotMapped]
        public string deviceId { get; set; }
        [NotMapped]
        public HumanDangerTrainingBaseEntity HumanDangerTraining { get; set; }
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            //this.MeetingId = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            //this.MeetingId = keyValue;
        }
        #endregion
    }

    public class MeetingAndJobEntity
    {
        [Column("MEETINGJOBID")]
        public string MeetingJobId { get; set; }
        [Column("STARTMEETINGID")]
        public string StartMeetingId { get; set; }
        [Column("ENDMEETINGID")]
        public string EndMeetingId { get; set; }
        [Column("JOBID")]
        public string JobId { get; set; }
        [Column("ISFINISHED")]
        public string IsFinished { get; set; }
        [Column("JOBUSERID")]
        public string JobUserId { get; set; }
        [Column("JOBUSER")]
        public string JobUser { get; set; }
        [Column("WORKPLANCONTENTID")]
        public string WorkPlanContentId { get; set; }
        [NotMapped]
        public List<JobUserEntity> JobUsers { get; set; }
        [NotMapped]
        public WorkmeetingEntity EndMeeting { get; set; }
    }

    public class JobDangerousEntity
    {
        [Column("JOBDANGEROUSID")]
        public string JobDangerousId { get; set; }
        [Column("DANGEROUSID")]
        public string DangerousId { get; set; }
        [Column("CONTENT")]
        public string Content { get; set; }
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
        [Column("JOBID")]
        public string JobId { get; set; }
        [NotMapped]
        public List<JobMeasureEntity> MeasureList { get; set; }
    }

    public class JobMeasureEntity
    {
        [Column("JOBMEASUREID")]
        public string JobMeasureId { get; set; }
        [Column("MEASUREID")]
        public string MeasureId { get; set; }
        [Column("CONTENT")]
        public string Content { get; set; }
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
        [Column("JOBDANGEROUSID")]
        public string JobDangerousId { get; set; }
    }
}