using System;
using System.Collections.Generic;
using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PerformanceManage
{
    /// <summary>
    /// 绩效考评配置表  特殊计算表
    /// </summary>
    [Table("WG_PERFORMANCEMETHOD_SECOND")]
    public class PerformanceMethodSecondEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 工作方法
        /// </summary>	
        [Column("METHODWORK")]
        public string methodwork { get; set; }
        /// <summary>
        /// 列明
        /// </summary>	
        [Column("NAME")]
        public string name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>	
        [Column("SORT")]
        public int sort { get; set; }

        /// <summary>
        /// 列id
        /// </summary>	
        [Column("PERFORMANCETYPEID")]
        public string performancetypeid { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {

        }
        #endregion
    }
}
