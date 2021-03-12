using System;
using System.Collections.Generic;
using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PerformanceManage
{
    /// <summary>
    /// 绩效考评配置表 参与人员
    /// </summary>
    [Table("WG_PERFORMANCEPERSON_SECOND")]
    public class PerformancePersonSecondEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>	
        [Column("MAKEUSER")]
        public string makeuser { get; set; }
        /// <summary>
        /// 排序
        /// </summary>	
        [Column("MAKEUSERID")]
        public string makeuserid { get; set; }
        /// <summary>
        /// 时间
        /// </summary>	
        [Column("CREATETIME")]
        public DateTime? createtime { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>	
        [Column("DEPARTMENTID")]
        public string departmentid { get; set; }
        /// <summary>
        /// 时间
        /// </summary>	
        [Column("MODIFYDATE")]
        public DateTime?  modifydate { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>	
        [Column("MODIFYUSER")]
        public string modifyuser { get; set; }
        /// <summary>
        /// 修改人id
        /// </summary>	
        [Column("MODIFYUSERID")]
        public string modifyuserid { get; set; }

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
