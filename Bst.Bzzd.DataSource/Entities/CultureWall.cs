using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    /// <summary>
    /// 班组文化墙
    /// </summary>
    [Table("WG_CULTUREWALL")]
    public class CultureWall
    {
        [Key]
        [Column("WALLINFOID")]
        public Guid wallinfoid { get; set; }

        [StringLength(36)]
        [Column("DEPARTMENTID")]
        public string departmentid { get; set; }

        [StringLength(100)]
        [Column("DEPARTMENTNAME")]
        public string departmentname { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        [MaxLength]
        [Column("SUMMARY")]
        public string summary { get; set; }

        /// <summary>
        /// 简介编辑时间
        /// </summary>
        [Column("SUMMARYDATE")]
        public DateTime? summarydate { get; set; }

        /// <summary>
        /// 口号
        /// </summary>
        [MaxLength]
        [Column("SLOGAN")]
        public string slogan { get; set; }

        /// <summary>
        /// 口号编辑时间
        /// </summary>
        [Column("SLOGANDATE")]
        public DateTime? slogandate { get; set; }
        /// <summary>
        /// 愿景
        /// </summary>
        [MaxLength]
        [Column("VISION")]
        public string vision { get; set; }

        /// <summary>
        /// 愿景编辑时间
        /// </summary>
        [Column("VISIONDATE")]
        public DateTime? visiondate { get; set; }
        /// <summary>
        /// 安全理念
        /// </summary>
        [MaxLength]
        [Column("CONCEPT")]
        public string concept { get; set; }
        /// <summary>
        /// 安全理念编辑时间
        /// </summary>
        [Column("CONCEPTDATE")]
        public DateTime? conceptdate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CREATETIME")]
        public DateTime? createtime { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [StringLength(50)]
        [Column("CREATEUSERID")]
        public string createuserid { get; set; }
    }

}
