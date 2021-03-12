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
    /// 描 述：排班关联部门
    /// </summary>
    [Table("wg_workorder")]
    public class WorkOrderEntity : BaseEntity
    {
        [Column("workorderid")]
        public string WorkOrderId { get; set; }
        [Column("starttime")]
        public DateTime StartTime { get; set; }
        [Column("endtime")]
        public DateTime EndTime { get; set; }

        [Column("usetime")]
        public DateTime useTime { get; set; }

        [Column("createtime")]
        public DateTime CreateTime { get; set; }
        [Column("createuserid")]
        public string CreateUserId { get; set; }

        [Column("bookmarks")]
        public string bookmarks { get; set; }
        [Column("isweek")]
        public bool isweek { get; set; }
        [Column("starttimespan")]
        public int StartTimeSpan { get; set; }
        [Column("endtimespan")]
        public int EndTimeSpan { get; set; }

        [Column("workname")]
        public string WorkName { get; set; }
        [Column("settingid")]
        public string settingid { get; set; }

        [Column("ordersort")]
        public int OrderSort { get; set; }

        [Column("setupId")]
        public string setupId { get; set; }

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.WorkOrderId = Guid.NewGuid().ToString();
            this.CreateTime = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.WorkOrderId = keyValue;

        }
        #endregion
    }

    public class WorkDeparmentList
    {
        public string DepartmentId { get; set; }
        public string FullName { get; set; }

        public int selectValue { get; set; }

    }
    public class WorksetList
    {
        public string text { get; set; }
        public string value { get; set; }

        public int sort { get; set; }

    }
    /// <summary>
    /// 排班展示
    /// </summary>
    public class WorkOrderList
    {
        public string WorkOrderId { get; set; }
        public string FullName { get; set; }
        public string TimeStr { get; set; }
        public int OrderSort { get; set; }
        public DateTime startTime { get; set; }
        public string bookMarks { get; set; }
        public string DeparMentId { get; set; }
    }
    public class WorkOrder
    {
        public string WorkOrderId { get; set; }
        public DateTime startTime { get; set; }
        public string startSpan { get; set; }
        public string endSpan { get; set; }
        public string SettingName { get; set; }
        public string bookMarks { get; set; }
        public DateTime useTime { get; set; }

        public int OrderSort { get; set; }
        public DateTime endTime { get; set; }
        public string DeparMentId { get; set; }

        public string FullName { get; set; }
        public bool IsWeek { get; set; }
        public bool Isbool { get; set; }

    }

}
