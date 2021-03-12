using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections.Generic;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    [Table("wg_meetingsignin")]
    public class MeetingSigninEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("signinid")]
        public string SigninId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("meetingid")]
        public string MeetingId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("userid")]
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("personname")]
        public string PersonName { get; set; }
        /// <summary>
        /// 出勤
        /// </summary>
        [Column("issigned")]
        public bool IsSigned { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("mentalcondition")]
        public string MentalCondition { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("closingcondition")]
        public string ClosingCondition { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createdate")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 缺勤原因
        /// </summary>
        [Column("Reason")]
        public string Reason { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("ReasonRemark")]
        public string ReasonRemark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        //public WorkmeetingEntity WorkMeeting { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.MeetingId = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.MeetingId = keyValue;
        }
        #endregion
    }

    public class SigninEntity
    {
        public string MeetingId { get; set; }
        public string MeetingType { get; set; }
        public bool IsSignin { get; set; }
        public string Reason { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public string OtherMeetingId { get; set; }
        public DateTime CreateDate { get; set; }
        public string ReasonRemark { get; set; }
    }

    public class SinginUserEntity
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<SigninEntity> Signins { get; set; }
    }

    public class UnSignRecordEntity
    {
        public string UnSignRecordId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime UnSignDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Reason { get; set; }
        public string ReasonRemark { get; set; }
        public float Hours { get; set; }
    }
}