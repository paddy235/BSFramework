using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：编号规则格式
    /// </summary>
    public class CodeRuleFormatEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 编号规则格式主键
        /// </summary>	
        [Column("RULEFORMATID")]
        public string RuleFormatId { get; set; }
        /// <summary>
        /// 编码规则主键
        /// </summary>		
         [Column("RULEID")]
        public string RuleId { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>		
         [Column("ITEMTYPE")]
        public int? ItemType { get; set; }
        /// <summary>
        /// 项目类型名称
        /// </summary>
        [Column("ITEMTYPENAME")]
        public string ItemTypeName { get; set; }
        /// <summary>
        /// 格式化字符串
        /// </summary>		
         [Column("FORMATSTR")]
        public string FormatStr { get; set; }
        /// <summary>
        /// 步长
        /// </summary>	
         [Column("STEPVALUE")]
        public int? StepValue { get; set; }
        /// <summary>
        /// 初始值
        /// </summary>	
         [Column("INITVALUE")]
        public int? InitValue { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>	
         [Column("SORTCODE")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>	
        [Column("DELETEMARK")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>	
        [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
         [Column("DESCRIPTION")]
        public string Description { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.RuleFormatId = Guid.NewGuid().ToString();
            this.DeleteMark = 0;
            this.EnabledMark = 1;
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