
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace System
{
    /// <summary>
    /// 班组终端屏保
    /// </summary>
    [Table("wg_screensaver")]
    public class ScreensaverEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("FileId")]
        [Description("")]
        public string FileId { get; set; }

        /// <summary>
        /// 班组ID
        /// </summary>
        [Column("DeptId")]
        [Description("班组ID")]
        public string DeptId { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        [Column("DeptName")]
        [Description("班组名称")]
        public string DeptName { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        [Column("FileName")]
        [Description("文件名称")]
        public string FileName { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Column("FileType")]
        [Description("文件类型")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件地址
        /// </summary>
        [Column("FilePath")]
        [Description("文件地址")]
        public string FilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERID")]
        [Description("")]
        public string CREATEUSERID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERNAME")]
        [Description("")]
        public string CREATEUSERNAME { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        [Description("")]
        public DateTime CREATEDATE { get; set; }

    }
}
