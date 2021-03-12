using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EvaluateAbout
{
    /// <summary>
    /// 考评范围表
    /// </summary>
    [Table("wg_EvaluatScope")]
   public  class EvaluateScopeEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        /// <returns></returns>
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 考评Id
        /// </summary>
        /// <returns></returns>
        [Column("EvaluateId")]
        public string EvaluateId { get; set; }
        /// <summary>
        /// 部门Id，逗号隔开
        /// </summary>
        /// <returns></returns>
        [Column("DeptId")]
        public string DeptId { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK1")]
        public string BK1 { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK2")]
        public string BK2 { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK3")]
        public string BK3 { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK4")]
        public string BK4 { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        #endregion
    }
}
