using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    [Table("WG_DANGERCATEGORY")]
    public class DangerCategoryEntity
    {
        [Column("CATEGORYID")]
        [StringLength(36)]
        public string CategoryId { get; set; }
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        [Column("PARENTCATEGORYID")]
        [StringLength(36)]
        public string ParentCategoryId { get; set; }
        [Column("SORT")]
        public int? Sort { get; set; }
    }

    public class DangerMeasureEntity
    {
        public string MeasureId { get; set; }
        public string CategoryId { get; set; }
        public string Category { get; set; }
        public string DangerReason { get; set; }
        public string MeasureContent { get; set; }
        public string OperateUser { get; set; }
        public DateTime OperateTime { get; set; }
        public string OperateUserId { get; set; }
    }
}
