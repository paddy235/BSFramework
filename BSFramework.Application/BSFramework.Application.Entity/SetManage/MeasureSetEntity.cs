using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SetManage
{
    /// <summary>
    /// 描 述：防范措施
    /// </summary>
    [Table("WG_MEASURESET")]
    public class MeasureSetEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 危险因素外键ID
        /// </summary>
        [Column("RISKFACTORID")]
        public string RiskFactorId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 防范措施
        /// </summary>
        [Column("CONTENT")]
        [Display(Name = "防范措施"),
            Required(AllowEmptyStrings = false, ErrorMessage = "请输入防范措施"),
            MinLength(1),
            MaxLength(500)]
        public string Content { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Column("SORT")]
        public int? Sort { get; set; }

        #endregion
    }
}
