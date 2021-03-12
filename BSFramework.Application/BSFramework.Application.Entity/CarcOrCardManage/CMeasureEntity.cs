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
    [Table("WG_CMEASURES")]
    public  class CMeasureEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id 
        /// </summary>	
        [Key]
        [Column("ID")]
        public string Id { get; set; }
    
        /// <summary>
        /// 采取的安全防范措施
        /// </summary>	

        [Column("MEASURE")]
        public string Measure { get; set; }


        /// <summary>
        ///创建时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// carc  card id
        /// </summary>	

        [Column("CMID")]
        public string Cmid { get; set; }
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
