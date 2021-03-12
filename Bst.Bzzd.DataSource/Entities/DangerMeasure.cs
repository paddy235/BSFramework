using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Bst.Bzzd.DataSource.Entities
{
    [Table("WG_DANGERMEASURE")]
    public class DangerMeasure
    {
        [Key]
        [Column("MEASUREID")]
        [StringLength(36)]
        public string MeasureId { get; set; }
        [StringLength(200)]
        [Column("DANGERREASON")]
        public string DangerReason { get; set; }
        [StringLength(500)]
        [Column("MEASURECONTENT")]
        public string MeasureContent { get; set; }
        [StringLength(36)]
        [Column("OPERATEUSERID")]
        public string OperateUserId { get; set; }
        [StringLength(30)]
        [Column("OPERATEUSER")]
        public string OperateUser { get; set; }
        [Column("OPERATETIME")]
        public DateTime OperateTime { get; set; }
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public DangerCategory Category { get; set; }
    }
}
