using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 工作标准
    /// </summary>
    [Table("wg_workstandard")]
    public class WorkStandardEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("Id")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        [Column("ModuleId")]
        [Description("模块ID")]
        public string ModuleId { get; set; }

        /// <summary>
        /// 模块编码
        /// </summary>
        [Column("ModuleCode")]
        [Description("模块编码")]
        public string ModuleCode { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        [Column("ModuleName")]
        [Description("模块名称")]
        public string ModuleName { get; set; }

        /// <summary>
        /// 说明
        /// </summary>
        [Column("Content")]
        [Description("说明")]
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CreateTime")]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        [NotMapped]
        public IEnumerable<FileInfoEntity> FileList { get; set; }

    }
}
