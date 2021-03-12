using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：日程管理
    /// </summary>
    public class ScheduleEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 日程主键
        /// </summary>
        /// <returns></returns>
         [Column("SCHEDULEID")]
        public string ScheduleId { get; set; }
        /// <summary>
        /// 日程名称
        /// </summary>
        /// <returns></returns>
          [Column("SCHEDULENAME")]
        public string ScheduleName { get; set; }
        /// <summary>
        /// 日程内容
        /// </summary>
        /// <returns></returns>、
         [Column("SCHEDULECONTENT")]
        public string ScheduleContent { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        /// <returns></returns>
       [Column("CATEGORY")]
        public string Category { get; set; }
        /// <summary>
        /// 开始日期
        /// </summary>
        /// <returns></returns>
       [Column("STARTDATE")]
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        /// <returns></returns>
         [Column("STARTTIME")]
        public string StartTime { get; set; }
        /// <summary>
        /// 结束日期
        /// </summary>
        /// <returns></returns>
         [Column("ENDDATE")]
        public DateTime? EndDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        /// <returns></returns>
         [Column("ENDTIME")]
        public string EndTime { get; set; }
        /// <summary>
        /// 提前提醒
        /// </summary>
        /// <returns></returns>
         [Column("EARLY")]
        public int? Early { get; set; }
        /// <summary>
        /// 邮件提醒
        /// </summary>
        /// <returns></returns>
         [Column("ISMAILALERT")]
        public int? IsMailAlert { get; set; }
        /// <summary>
        /// 手机提醒
        /// </summary>
          [Column("ISMOBILEALERT")]
        public int? IsMobileAlert { get; set; }
        /// <summary>
        /// 微信提醒
        /// </summary>
        /// <returns></returns>
        [Column("ISWECHATALERT")]
        public int? IsWeChatAlert { get; set; }
        /// <summary>
        /// 日程状态
        /// </summary>
        /// <returns></returns>
         [Column("SCHEDULESTATE")]
        public int? ScheduleState { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
         [Column("DELETEMARK")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        /// <returns></returns>
          [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
         [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        /// <returns></returns>
          [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        /// <returns></returns>
         [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
         [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
         [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ScheduleId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ScheduleId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}