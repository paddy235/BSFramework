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
    [Table("WG_DEVICEINSPECTION")]
    public class DeviceInspectionEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 检查表名称 唯一
        /// </summary>
        [Column("INSPECTIONNAME")]
        [Description("检查表名称 唯一")]
        public string InspectionName { get; set; }

        /// <summary>
        /// 设备系统
        /// </summary>
        [Column("DEVICESYSTEM")]
        [Description("设备系统")]
        public string DeviceSystem { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DEPTID")]
        [Description("")]
        public string DeptId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DEPTCODE")]
        [Description("")]
        public string DeptCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("DEPTNAME")]
        [Description("")]
        public string DeptName { get; set; }

        /// <summary>
        /// 记录创建时间
        /// </summary>
        [Column("CREATEDATE")]
        [Description("记录创建时间")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 记录创建人编码
        /// </summary>
        [Column("CREATEUSERID")]
        [Description("记录创建人编码")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 记录创建人姓名
        /// </summary>
        [Column("CREATEUSERNAME")]
        [Description("记录创建人姓名")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("MODIFYDATE")]
        [Description("修改时间")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 最近一次修改人编码
        /// </summary>
        [Column("MODIFYUSERID")]
        [Description("最近一次修改人编码")]
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 最近一次修改人姓名
        /// </summary>
        [Column("MODIFYUSERNAME")]
        [Description("最近一次修改人姓名")]
        public string ModifyUserName { get; set; }


        [NotMapped]
        public List<DeviceInspectionItemEntity> DeviceInspectionItem { get; set; }
        public new void Create()
        {
            Operator user = OperatorProvider.Provider.Current();
            Id = Guid.NewGuid().ToString();
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
