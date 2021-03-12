using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PerformanceManage
{
    /// <summary>
    /// 绩效考评数据表
    /// </summary>
    [Table("WG_PERFORMANCE_SECOND")]
    public class PerformanceSecondEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        [Column("PERFORMANCEID")]
        public string performanceid { get; set; }
        /// <summary>
        /// 数据集合 例如：1,2,3
        /// </summary>	
        [Column("SCORE")]
        public string score { get; set; }
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
        [Column("DEPARMENTID")]
        public string deparmentid { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>	
        [Column("USERID")]
        public string userid { get; set; }
        /// <summary>
        /// 标题名称序号例如 1,3,4
        /// </summary>	
        [Column("SORT")]
        public string sort { get; set; }
        /// <summary>
        /// 关联标题表id
        /// </summary>	
        [Column("TITLEID")]
        public string titleid { get; set; }
        [Column("PLANER")]
        public String planer { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>	
        [Column("DEPARMENTNAME")]
        public string deparmentname { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>	
        [Column("USERNAME")]
        public string username { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        [Column("QUARTERS")]
        public String quarters { get; set; }
        [Column("PHOTO")]
        public String photo { get; set; }
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
