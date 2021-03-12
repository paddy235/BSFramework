using BSFramework.Application.Code;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.SafetyScore
{
    /// <summary>
    /// 安全卫士_计分规则
    /// </summary>
    [Table("WG_ACCOUNTRULE")]
    public class AccountRuleEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 分值
        /// </summary>
        [Column("SCORE")]
        [Description("分值")]
        public decimal Score { get; set; }

        /// <summary>
        /// 标准
        /// </summary>
        [Column("STANDARD")]
        [Description("标准")]
        public string Standard { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>
        [Column("SORT")]
        [Description("排序号")]
        public int Sort { get; set; }

        /// <summary>
        /// 计分类型
        /// </summary>
        [Column("SCORETYPE")]
        [Description("计分类型")]
        public string ScoreType { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [Column("ISOPEN")]
        [Description("是否启用")]
        public int IsOpen { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("REMARK")]
        [Description("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        [Description("")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERID")]
        [Description("")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERNAME")]
        [Description("")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYDATE")]
        [Description("")]
        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYUSERID")]
        [Description("")]
        public string ModifyUserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("MODIFYUSERNAME")]
        [Description("")]
        public string ModifyUserName { get; set; }

        public override void Create()
        {
            Operator user = OperatorProvider.Provider.Current();
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }

        public void Modify()
        {
            Operator user = OperatorProvider.Provider.Current();
            ModifyDate = DateTime.Now;
            ModifyUserId = user.UserId;
            ModifyUserName = user.UserName;
        }

    }
}
