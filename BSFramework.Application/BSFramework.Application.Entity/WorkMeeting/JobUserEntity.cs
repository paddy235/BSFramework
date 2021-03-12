using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.ComponentModel;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// 描 述：作业人
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
        /// 0 初始
        /// 1 危险辨识保存
        /// 2 危险辨识提交
        /// 3 落实保存
        /// 4 提交
        /// </summary>
        [Column("DANGERSTATUS")]
        public int DangerStatus { get; set; }
        [Column("JOBID")]
        public string JobId { get; set; }

        /// <summary>
        /// 工时
        /// </summary>
        [Description("工时")]
        [Column("TASKHOUR")]
        public decimal? TaskHour { get; set; }

        /// <summary>
        /// 签到状态
        /// </summary>
        [NotMapped]
        public bool? IsSignin { get; set; }
    }
}