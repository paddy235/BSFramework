using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.CarcOrCardManage
{
    /// <summary>
    /// 描 述：Carc 手袋卡-风险标识
    /// </summary>
    [Table("WG_CDANGEROUS")]
    public  class CDangerousEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id 
        /// </summary>	
        [Key]
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 危险名称
        /// </summary>	

        [Column("DANGERNAME")]
        public string DangerName { get; set; }
        /// <summary>
        /// 危险描述
        /// </summary>	

        [Column("DANGERSOURCE")]
        public string DangerSource { get; set; }


        /// <summary>
        /// 是否可控 0 false 1 ture
        /// </summary>	

        [Column("ISSAFE")]
        public int? IsSafe { get; set; }

        /// <summary>
        /// carc  card id
        /// </summary>	

        [Column("CID")]
        public string Cid { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }

        /// <summary>
        /// 措施
        /// </summary>
        [NotMapped]
        public List<CMeasureEntity> Measure { get; set; }
        #endregion

        #region 扩展操作

        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {

        }
        #endregion
    }
}
