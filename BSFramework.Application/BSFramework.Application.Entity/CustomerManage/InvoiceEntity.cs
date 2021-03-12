using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.CustomerManage
{
    /// <summary>
    /// 描 述：开票信息
    /// </summary>
     [Table("CLIENT_INVOICE")]
    public class InvoiceEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 开票信息主键
        /// </summary>
        /// <returns></returns>
         [Column("INVOICEID")]
         public string InvoiceId { get; set; }
        /// <summary>
        /// 客户主键
        /// </summary>
        /// <returns></returns>
         [Column("CUSTOMERID")]
         public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        /// <returns></returns>
         [Column("CUSTOMERCODE")]
         public string CustomerCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// <returns></returns>
         [Column("CUSTOMERNAME")]
         public string CustomerName { get; set; }
        /// <summary>
        /// 开票信息
        /// </summary>
        /// <returns></returns>
         [Column("INVOICECONTENT")]
         public string InvoiceContent { get; set; }
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
            this.InvoiceId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.InvoiceId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}