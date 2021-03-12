using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.CustomerManage
{
    /// <summary>
    /// 描 述：商机信息
    /// </summary>
    [Table("CLIENT_CHANCE")]
    public class ChanceEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 商机主键
        /// </summary>
        /// <returns></returns>
        [Column("CHANCEID")]
        public string ChanceId { get; set; }
        /// <summary>
        /// 商机编号
        /// </summary>
        /// <returns></returns>
        [Column("ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 商机名称
        /// </summary>
        /// <returns></returns>
        [Column("FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 商机类别
        /// </summary>
        /// <returns></returns>
        [Column("CHANCETYPEID")]
        public string ChanceTypeId { get; set; }
        /// <summary>
        /// 商机来源
        /// </summary>
        /// <returns></returns>
        [Column("SOURCEID")]
        public string SourceId { get; set; }
        /// <summary>
        /// 商机阶段
        /// </summary>
        /// <returns></returns>
        [Column("STAGEID")]
        public string StageId { get; set; }
        /// <summary>
        /// 成功率
        /// </summary>
        /// <returns></returns>
        [Column("SUCCESSRATE")]
        public decimal? SuccessRate { get; set; }
        /// <summary>
        /// 预计金额
        /// </summary>
        /// <returns></returns>
        [Column("AMOUNT")]
        public decimal? Amount { get; set; }
        /// <summary>
        /// 预计利润
        /// </summary>
        /// <returns></returns>
        [Column("PROFIT")]
        public decimal? Profit { get; set; }
        /// <summary>
        /// 销售费用
        /// </summary>
        /// <returns></returns>
           [Column("SALECOST")]
        public decimal? SaleCost { get; set; }
        /// <summary>
        /// 预计成交时间
        /// </summary>
        /// <returns></returns>
           [Column("DEALDATE")]
        public DateTime? DealDate { get; set; }
        /// <summary>
        /// 转换客户
        /// </summary>
        /// <returns></returns>
           [Column("ISTOCUSTOM")]
        public int? IsToCustom { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        /// <returns></returns>
           [Column("COMPANYNAME")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 公司性质
        /// </summary>
        /// <returns></returns>
          [Column("COMPANYNATUREID")]
        public string CompanyNatureId { get; set; }
        /// <summary>
        /// 公司地址
        /// </summary>
        /// <returns></returns>
         [Column("COMPANYADDRESS")]
        public string CompanyAddress { get; set; }
        /// <summary>
        /// 公司网站
        /// </summary>
        /// <returns></returns>
          [Column("COMPANYSITE")]
        public string CompanySite { get; set; }
        /// <summary>
        /// 公司情况
        /// </summary>
        /// <returns></returns>
         [Column("COMPANYDESC")]
        public string CompanyDesc { get; set; }
        /// <summary>
        /// 所在省份
        /// </summary>
        /// <returns></returns>
           [Column("PROVINCE")]
        public string Province { get; set; }
        /// <summary>
        /// 所在城市
        /// </summary>
        /// <returns></returns>
        [Column("CITY")]
        public string City { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        /// <returns></returns>
           [Column("CONTACTS")]
        public string Contacts { get; set; }
        /// <summary>
        /// 手机
        /// </summary>
        /// <returns></returns>
          [Column("MOBILE")]
        public string Mobile { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        /// <returns></returns>
           [Column("TEL")]
        public string Tel { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        /// <returns></returns>
          [Column("FAX")]
        public string Fax { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        /// <returns></returns>
          [Column("QQ")]
        public string QQ { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        /// <returns></returns>
        [Column("EMAIL")]
        public string Email { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        /// <returns></returns>
          [Column("WECHAT")]
        public string Wechat { get; set; }
        /// <summary>
        /// 爱好
        /// </summary>
        /// <returns></returns>
          [Column("HOBBY")]
        public string Hobby { get; set; }
        /// <summary>
        /// 跟进人员Id
        /// </summary>
        /// <returns></returns>
          [Column("TRACEUSERID")]
        public string TraceUserId { get; set; }
        /// <summary>
        /// 跟进人员
        /// </summary>
        /// <returns></returns>
         [Column("TRACEUSERNAME")]
        public string TraceUserName { get; set; }
        /// <summary>
        /// 商机状态编码（0-作废）
        /// </summary>
        /// <returns></returns>
         [Column("CHANCESTATE")]
        public int? ChanceState { get; set; }
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
            this.ChanceId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.ModifyDate = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ChanceId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}