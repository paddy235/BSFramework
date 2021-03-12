using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 描 述：工作流委托规则与工作流模板对应表
    /// </summary>
    [Table("WF_DELEGATERULESCHEMEINFO")]
    public class WFDelegateRuleSchemeInfoEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 规则Id
        /// </summary>
        [Column("DELEGATERULEID")]
        public string DelegateRuleId { get; set; }
        /// <summary>
        /// 模板Id
        /// </summary>
        [Column("SCHEMEINFOID")]
        public string SchemeInfoId { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
        }
        #endregion
    }
}
