using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Code;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// 描 述：班次
    /// </summary>
    [Table("WG_WORKSETTING")]
    public class WorkSettingEntity : BaseEntity
    {
        [Column("WORKSETTINGID")]
        public string WorkSettingId { get; set; }
        [Column("NAME")]
        public string Name { get; set; }
        [Column("STARTTIME")]
        public DateTime StartTime { get; set; }
        [Column("ENDTIME")]
        public DateTime EndTime { get; set; }
        [Column("TIMELENGTH")]
        public string TimeLength { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
        [Column("WORKSETNAME")]
        public string WorkSetName { get; set; }
        [Column("BOOKMARKS")]
        public string BookMarks { get; set; }
        [Column("WORKSETTYPE")]
        public int WorkSetType { get; set; }
        [Column("WORKSTATE")]
        public bool WorkState { get; set; }
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        [Column("STARTTIMESPAN")]
        public int StartTimeSpan { get; set; }
        [Column("AFTERTIME")]
        public int AfterTime { get; set; }
        [Column("ENDTIMESPAN")]
        public int EndTimeSpan { get; set; }
        [Column("WORKSETUPID")]
        public string WorkSetupId { get; set; }
        [Column("DEPARMENTID")]
        public string DeparMentId { get; set; }

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.WorkSettingId = Guid.NewGuid().ToString();
            this.CreateTime = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.WorkState = false;
            this.StartTimeSpan = 30;
            this.EndTimeSpan = 30;

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.WorkSettingId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
        }
        #endregion
    }
}