using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SweepManage
{
    /// <summary>
    /// 保洁管理保洁项
    /// </summary>
    [Table("WG_SWEEPITTEM")]
    public class SweepItemEntity : BaseEntity
    {
        /// <summary>
        ///主键id
        /// </summary>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        ///名称
        /// </summary>
        [Column("NAME")]
        public string Name { get; set; }

        /// <summary>
        ///是否勾选
        /// </summary>
        [Column("STATE")]
        public bool State { get; set; }
        /// <summary>
        ///保洁管理id
        /// </summary>
        [Column("SWEEPID")]
        public string SweepId { get; set; }
        /// <summary>
        ///序号
        /// </summary>
        [Column("SORT")]
        public string Sort { get; set; }

    }
}
