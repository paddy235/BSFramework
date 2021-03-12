using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.PerformanceManage
{
    /// <summary>
    /// 绩效考评月份标题表
    /// </summary>
    [Table("WG_PERFORMANCETITLE_SECOND")]
    public class PerformancetitleSecondEntity : BaseEntity
    {

        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        [Column("TITLEID")]
        public string titleid { get; set; }

        /// <summary>
        /// 标题名称例如 绩效,分数,扣钱
        /// </summary>	
        [Column("NAME")]
        public string name { get; set; }

        /// <summary>
        /// 使用时间（报表时间）
        /// </summary>	
        [Column("USETIME")]
        public DateTime usetime { get; set; }
        /// <summary>
        /// 使用时间 年
        /// </summary>	
        [Column("USEYEAR")]
        public string useyear { get; set; }
        /// <summary>
        /// 使用时间 月
        /// </summary>	
        [Column("USEMONTH")]
        public string usemonth { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("DEPARTMENTID")]
        public string departmentid { get; set; }
        /// <summary>
        /// 序号
        /// </summary>	
        [Column("SORT")]
        public string sort { get; set; }

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
