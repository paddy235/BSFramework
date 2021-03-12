using System;
using System.Collections.Generic;
using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PerformanceManage
{
    /// <summary>
    /// 绩效考评配置表
    /// </summary>
    [Table("WG_PERFORMANCESETUP")]
    public class PerformancesetupEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Column("PERFORMANCETYPEID")]
        public string performancetypeid { get; set; }
        /// <summary>
        /// 名称
        /// </summary>	
        [Column("NAME")]
        public string name { get; set; }
        /// <summary>
        /// 排序
        /// </summary>	
        [Column("SORT")]
        public int sort { get; set; }
        /// <summary>
        /// 时间
        /// </summary>	
        [Column("CREATETIME")]
        public DateTime? createtime { get; set; }
        /// <summary>
        /// 类型
        /// </summary>	
        [Column("TYPE")]
        public int type { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>	
        [Column("ISUSE")]
        public bool isuse { get; set; }

        /// <summary>
        /// 是否能修改
        /// </summary>	
        [Column("ISUPDATE")]
        public bool isupdate { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("DEPARTMENTID")]
        public string departmentid { get; set; }

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
