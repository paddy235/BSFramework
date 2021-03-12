using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：编号规则种子
    /// </summary>
    public class CodeRuleSeedEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 编号规则种子主键
        /// </summary>		
        [Column("RULESEEDID")]
        public string RuleSeedId { get; set; }
        /// <summary>
        /// 编码规则主键
        /// </summary>	
         [Column("RULEID")]
        public string RuleId { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>	
         [Column("USERID")]
        public string UserId { get; set; }
        /// <summary>
        /// 种子值
        /// </summary>	
        [Column("SEEDVALUE")]
        public int? SeedValue { get; set; }
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
            this.RuleSeedId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }

        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}