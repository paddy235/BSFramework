using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PerformanceManage
{
    /// <summary>
    /// 是否提交绩效
    /// </summary>
    [Table("WG_PERFORMANCEUP")]
    public class PerformanceupEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>	
        [Column("ID")]
        public string id { get; set; }
        /// <summary>
        /// 数据集合 例如：1,2,3
        /// </summary>	
        [Column("TITLEID")]
        public string titleid { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("DEPARTMENTID")]
        public string departmentid { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("DEPARTMENTNAME")]
        public string departmentname { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("DEPTCODE")]
        public string deptcode { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>	
        [Column("PARENTNAME")]
        public string parentname { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>	
        [Column("PARENTID")]
        public string parentid { get; set; }
        /// <summary>
        /// 上级部门
        /// </summary>	
        [Column("PARENTCODE")]
        public string parentcode { get; set; }
        /// <summary>
        /// 使用时间（报表时间）
        /// </summary>	
        [Column("ISUP")]
        public bool isup { get; set; }
        /// <summary>
        /// 使用时间（报表时间）
        /// </summary>	
        [Column("USETIME")]
        public string usetime { get; set; }
        /// <summary>
        /// 使用时间（报表时间）
        /// </summary>	
        [Column("USEYEAR")]
        public string useyear { get; set; }

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
