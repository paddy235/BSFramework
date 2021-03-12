using BSFramework.Application.Code;
using BSFramework.Application.Entity.BaseManage;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.DeviceInspection
{
    /// <summary>
    /// 设备巡回检查记录表
    /// </summary>
    [Table("WG_DEVICEINSPECTIONRECORD")]
    public class InspectionRecordEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 主键
        /// </summary>
        [Column("JOBID")]
        [Description("任务的主键")]
        public string JobId { get; set; }

        /// <summary>
        /// 设备巡回检查表的主键
        /// </summary>
        [Column("DEVICEID")]
        [Description("设备巡回检查表的主键")]
        public string DeviceId { get; set; }

        /// <summary>
        /// 设备系统名称
        /// </summary>
        [Column("DEVICESYSTEM")]
        [Description("设备系统名称")]
        public string DeviceSystem { get; set; }

        /// <summary>
        /// 检查表名称
        /// </summary>
        [Column("INSPECTIONNAME")]
        [Description("检查表名称")]
        public string InspectionName { get; set; }

        /// <summary>
        /// 检查记录
        /// </summary>
        [Column("RECORD")]
        [Description("检查记录")]
        public string Record { get; set; }

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
        [Column("CREATEUSERID")]
        [Description("")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERNAME")]
        [Description("")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERDEPTID")]
        [Description("")]
        public string CreateUserDeptId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERDEPTCODE")]
        [Description("")]
        public string CreateUserDeptCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERDEPTNAME")]
        [Description("")]
        public string CreateUserDeptName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        [Description("")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 是否提交 0未提交  1已提交
        /// </summary>
        [Column("ISSUBMIT")]
        [Description("是否提交 0未提交  1已提交")]
        public int IsSubmit { get; set; }

        public new void Create(UserEntity user)
        {
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.RealName;
            CreateUserDeptCode = user.DepartmentCode;
            CreateUserDeptId = user.DepartmentId;
            CreateUserDeptName = user.DepartmentName;
        }
    }
}
