using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.CustomerManage
{
    /// <summary>
    /// 描 述：费用支出
    /// </summary>
    [Table("CLIENT_EXPENSES")]
    public class ExpensesEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 支出主键
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESID")]
        public string ExpensesId { get; set; }
        /// <summary>
        /// 支出日期
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESDATE")]
        public DateTime? ExpensesDate { get; set; }
        /// <summary>
        /// 支出金额
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESPRICE")]
        public decimal? ExpensesPrice { get; set; }
        /// <summary>
        /// 支出账户
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESACCOUNT")]
        public string ExpensesAccount { get; set; }
        /// <summary>
        /// 支出种类
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESTYPE")]
        public string ExpensesType { get; set; }
        /// <summary>
        /// 支出对象（1-公司支付；2-个人垫付）
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESOBJECT")]
        public int? ExpensesObject { get; set; }
        /// <summary>
        /// 经办人
        /// </summary>
        /// <returns></returns>
        [Column("MANAGERS")]
        public string Managers { get; set; }
        /// <summary>
        /// 支出摘要
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSESABSTRACT")]
        public string ExpensesAbstract { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
        [Column("DELETEMARK")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        /// <returns></returns>
        [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        /// <returns></returns>
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        /// <returns></returns>
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ExpensesId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.ExpensesObject = 1;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ExpensesId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}