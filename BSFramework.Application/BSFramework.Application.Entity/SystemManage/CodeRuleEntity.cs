using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：编号规则
    /// </summary>
    public class CodeRuleEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 编码规则主键
        /// </summary>
       [Column("RULEID")]
        public string RuleId { get; set; }
        /// <summary>
        /// 系统功能Id
        /// </summary>	
         [Column("MODULEID")]
        public string ModuleId { get; set; }
        /// <summary>
        /// 系统功能
        /// </summary>	
          [Column("MODULE")]
        public string Module { get; set; }
        /// <summary>
        /// 编号
        /// </summary>		
        [Column("ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>	
         [Column("FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 方式（1-可编辑、自动）
        /// </summary>		
        [Column("PMODE")]
        public int? pMode { get; set; }
        /// <summary>
        /// 当前流水号
        /// </summary>	
        [Column("CURRENTNUMBER")]
        public string CurrentNumber { get; set; }
        /// <summary>
        /// 规则格式Json
        /// </summary>	
        [Column("RULEFORMATJSON")]
        public string RuleFormatJson { get; set; }
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
        /// <summary>
        /// 创建日期
        /// </summary>		
          [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>	
         [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>		
         [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>	
         [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>		
         [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>	
          [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.RuleId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
            this.EnabledMark = 1;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.RuleId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}