using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.DeviceInspection
{
    /// <summary>
    /// 设备巡回检查项
    /// </summary>
    [Table("wg_deviceinspectionitemjob")]
    public class DeviceInspectionItemJobEntity : BaseEntity
    {
        /// <summary>
        /// 设备巡回检查项主键
        /// </summary>
        [Column("Id")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 设备巡回检查表主键
        /// </summary>
        [Column("DeviceId")]
        [Description("设备巡回检查表主键")]
        public string DeviceId { get; set; }

        /// <summary>
        /// 检查项目
        /// </summary>
        [Column("ItemName")]
        [Description("检查项目")]
        public string ItemName { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        [Column("Method")]
        [Description("方法")]
        public string Method { get; set; }

        /// <summary>
        /// 标准
        /// </summary>
        [Column("Standard")]
        [Description("标准")]
        public string Standard { get; set; }

        /// <summary>
        /// 检查项的检查结果Id，非数据库字段
        /// </summary>
        [NotMapped]
        public string ResultId { get; set; }

        /// <summary>
        /// 检查项的检查结果，非数据库字段
        /// </summary>
        [NotMapped]
        public string Result { get; set; }

    }
}
            