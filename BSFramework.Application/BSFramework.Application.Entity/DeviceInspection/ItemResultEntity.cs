using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.DeviceInspection
{
    /// <summary>
    /// 设备巡回检查记录各项的检查结果
    /// </summary>
    [Table("WG_INSPECTIONITEMRESULT")]
    public class ItemResultEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("ID")]
        [Description("")]
        public string Id { get; set; }

        /// <summary>
        /// 设备巡回检查记录的Id
        /// </summary>
        [Column("RECORDID")]
        [Description("设备巡回检查记录的Id")]
        public string RecordId { get; set; }

        /// <summary>
        /// 设备巡回检查记录项的ID
        /// </summary>
        [Column("ITEMID")]
        [Description("设备巡回检查记录项的ID")]
        public string ItemId { get; set; }

        /// <summary>
        /// 结果
        /// </summary>
        [Column("RESULT")]
        [Description("结果")]
        public string Result { get; set; }

    }
}
