using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.CustomerManage
{
    /// <summary>
    /// 描 述：现金余额
    /// </summary>
    [Table("CLIENT_CASHBALANCE")]
    public class CashBalanceEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 现金余额主键
        /// </summary>
        /// <returns></returns>
        [Column("CASHBALANCEID")]
        public string CashBalanceId { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        /// <returns></returns>
        [Column("OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// ExecutionDate
        /// </summary>
        /// <returns></returns>
        [Column("EXECUTIONDATE")]
        public DateTime? ExecutionDate { get; set; }
        /// <summary>
        /// CashAccount
        /// </summary>
        /// <returns></returns>
        [Column("CASHACCOUNT")]
        public string CashAccount { get; set; }
        /// <summary>
        /// Receivable
        /// </summary>
        /// <returns></returns>
        [Column("RECEIVABLE")]
        public decimal? Receivable { get; set; }
        /// <summary>
        /// Expenses
        /// </summary>
        /// <returns></returns>
        [Column("EXPENSES")]
        public decimal? Expenses { get; set; }
        /// <summary>
        /// Balance
        /// </summary>
        /// <returns></returns>
        [Column("BALANCE")]
        public decimal? Balance { get; set; }
        /// <summary>
        /// Abstract
        /// </summary>
        /// <returns></returns>
        [Column("ABSTRACT")]
        public string Abstract { get; set; }
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
            this.CashBalanceId = Guid.NewGuid().ToString();
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
            this.CashBalanceId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}