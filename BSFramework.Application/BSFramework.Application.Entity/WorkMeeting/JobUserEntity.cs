using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.ComponentModel;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// �� ������ҵ��
    /// </summary>
    [Table("WG_JOBUSER")]
    public class JobUserEntity : BaseEntity
    {
        [Column("JOBUSERID")]
        public string JobUserId { get; set; }
        [Column("USERID")]
        public string UserId { get; set; }
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("JOBTYPE")]
        public string JobType { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("MEETINGJOBID")]
        public string MeetingJobId { get; set; }
        [Column("SCORE")]
        public int? Score { get; set; }
        [NotMapped]
        public string ImageUrl { get; set; }
        [NotMapped]
        public string Photo { get; set; }
        [NotMapped]
        public string DangerDutyContent { get; set; }
        [NotMapped]
        public string Measure { get; set; }
        [NotMapped]
        public string Danger { get; set; }
        /// <summary>
        /// 0 ��ʼ
        /// 1 Σ�ձ�ʶ����
        /// 2 Σ�ձ�ʶ�ύ
        /// 3 ��ʵ����
        /// 4 �ύ
        /// </summary>
        [Column("DANGERSTATUS")]
        public int DangerStatus { get; set; }
        [Column("JOBID")]
        public string JobId { get; set; }

        /// <summary>
        /// ��ʱ
        /// </summary>
        [Description("��ʱ")]
        [Column("TASKHOUR")]
        public decimal? TaskHour { get; set; }

        /// <summary>
        /// ǩ��״̬
        /// </summary>
        [NotMapped]
        public bool? IsSignin { get; set; }
    }
}