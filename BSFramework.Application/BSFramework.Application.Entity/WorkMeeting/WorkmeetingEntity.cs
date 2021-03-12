using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.Activity;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// 描 述：班前班后会
    /// </summary>
    [Table("WG_WORKMEETING")]
    public class WorkmeetingEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("MEETINGID")]
        public string MeetingId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MEETINGTYPE")]
        public string MeetingType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MEETINGSTARTTIME")]
        public DateTime MeetingStartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MEETINGENDTIME")]
        public DateTime MeetingEndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("SHOULDJOIN")]
        public int ShouldJoin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ACTUALLYJOIN")]
        public int ActuallyJoin { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("REMARK")]
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("GROUPID")]
        public string GroupId { get; set; }
        [Column("GROUPNAME")]
        public string GroupName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("OTHERMEETINGID")]
        public string OtherMeetingId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ISOVER")]
        public bool IsOver { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("MEETINGPERSON")]
        public string MeetingPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("PERSONSTATE")]
        public string PersonState { get; set; }
        /// <summary>
        /// 班前会是否开始
        /// </summary>
        [Column("ISSTARTED")]
        public bool IsStarted { get; set; }
        [NotMapped]
        public bool? IsEvaluate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<MeetingJobEntity> Jobs { get; set; }
        [NotMapped]
        public IList<MeetingSigninEntity> Signins { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files2 { get; set; }
        [NotMapped]
        public List<UnSignRecordEntity> DutyPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public IList<ActivityEvaluateEntity> Evaluates { get; set; }
        [NotMapped]
        public string UserId { get; set; }
        [Column("SHOULDSTARTTIME")]
        public DateTime? ShouldStartTime { get; set; }
        [Column("SHOULDENDTIME")]
        public DateTime? ShouldEndTime { get; set; }
        [Column("MEETINGCODE")]
        public string MeetingCode { get; set; }
        [NotMapped]
        public bool? IsOver2 { get; set; }
        [NotMapped]
        public DateTime? OtherMeetingStartTime { get; set; }

       
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

    public class MeetingEntity
    {
        public string MeetingId { get; set; }
        public string Team { get; set; }
        public DateTime StartTime1 { get; set; }
        public DateTime EntTime1 { get; set; }
        public DateTime StartTime2 { get; set; }
        public DateTime EndTime2 { get; set; }
        public bool IsEvaluated { get; set; }
    }
    public class MeetingBookEntity
    {
        public string MeetingId { get; set; }
        public string  DeptName{ get; set; }
        public string DeptId { get; set; }
        public DateTime? TopStartTime { get; set; }
        public DateTime? TopEndTime { get; set; }
        public DateTime? AfterStartTime{ get; set; }
        public DateTime? AfterEndTime { get; set; }
        public IEnumerable<ActivityEvaluateEntity> Evaluates { get; set; }
    }




    public class KeyTimesEntity
    {
        public string UserId { get; set; }
        public double Times { get; set; }
    }

    public class Meeting2Entity
    {
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public int Job1 { get; set; }
        public int Job2 { get; set; }
        public int Job3 { get; set; }
        public int Job4 { get; set; }
        public int Job5 { get; set; }
        public string MeetingId { get; set; }
        public DateTime MeetingStartTime { get; set; }
        public string OtherMeetingId { get; set; }
        public int Pic1 { get; set; }
        public int Pic2 { get; set; }
        public string Video1 { get; set; }
        public string Video2 { get; set; }
    }
}