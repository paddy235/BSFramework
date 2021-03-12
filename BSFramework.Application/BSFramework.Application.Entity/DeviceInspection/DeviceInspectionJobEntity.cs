using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.DeviceInspection
{
    /// <summary>
    /// 设备巡检表
    /// </summary>
    [Table("wg_deviceinspectionjob")]
    public class DeviceInspectionJobEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("Id")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 检查表名称 唯一
        /// </summary>
        [Column("InspectionName")]
        [Description("检查表名称 唯一")]
        public string InspectionName { get; set; }

        /// <summary>
        /// 设备系统
        /// </summary>
        [Column("DeviceSystem")]
        [Description("设备系统")]
        public string DeviceSystem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DeptId")]
        [Description("")]
        public string DeptId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DeptCode")]
        [Description("")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 检查人
        /// </summary>
        [Column("WORKUSER")]
        [Description("检查人")]
        public string Workuser { get; set; }

        /// <summary>
        /// 检查人
        /// </summary>
        [Column("WORKUSERID")]
        [Description("检查人")]
        public string WorkuserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DeptName")]
        [Description("")]
        public string DeptName { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        [Column("CreateDate")]
        [Description("记录创建时间")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 记录创建人编码
        /// </summary>
        [Column("CreateUserId")]
        [Description("记录创建人编码")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 记录创建人姓名
        /// </summary>
        [Column("CreateUserName")]
        [Description("记录创建人姓名")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("ModifyDate")]
        [Description("修改时间")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 最近一次修改人编码
        /// </summary>
        [Column("ModifyUserId")]
        [Description("最近一次修改人编码")]
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 最近一次修改人姓名
        /// </summary>
        [Column("ModifyUserName")]
        [Description("最近一次修改人姓名")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 是否代办
        /// </summary>
        [Column("STATE")]
        [Description("是否代办")]
        public bool State { get; set; }
        /// <summary>
        /// 活动id
        /// </summary>
        [Column("MEETID")]
        [Description("活动id")]
        public string MeetId { get; set; }
        /// <summary>
        /// 任务id
        /// </summary>
        [Column("JOBID")]
        [Description("任务id")]
        public string JobId { get; set; }
        /// <summary>
        /// 记录id
        /// </summary>
        [Column("RECORDID")]
        [Description("记录id")]
        public string recordId { get; set; }

        [NotMapped]
        public List<DeviceInspectionItemJobEntity> DeviceInspectionItem { get; set; }
        public new void Create()
        {
            Operator user = OperatorProvider.Provider.Current();
            if (string.IsNullOrEmpty(this.Id))
            {
                Id = Guid.NewGuid().ToString();
            }

            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }

        public new void Modify()
        {
            Operator user = OperatorProvider.Provider.Current();
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }
    }
}
