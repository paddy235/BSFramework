using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 活动类型
    /// </summary>
    [Table("WG_ACTIVITYCATEGORY")]
    public class ActivityCategoryEntity : BaseEntity
    {
        /// <summary>
        ///  主键
        /// </summary>
        [Column("ACTIVITYCATEGORYID")]
        public string ActivityCategoryId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        [Column("ACTIVITYCATEGORY")]
        public string ActivityCategory { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATETIME")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSER")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        [Column("DEPTNAME")]
        public string deptname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public int? Total { get; set; }
        /// <summary>
        /// 班组ID
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public int? State { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string ActivityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public string ShowHtml { get; set; }

    }
}
