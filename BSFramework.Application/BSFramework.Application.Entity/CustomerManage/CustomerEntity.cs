using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.CustomerManage
{
    /// <summary>
    /// 描 述：客户信息
    /// </summary>
        [Table("CLIENT_CUSTOMER")]
    public class CustomerEntity : BaseEntity
    {
        #region 实体成员
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
          [Column("ENCODE")]
            public string EnCode { get; set; }
        /// <summary>
        /// 客户名称
        /// </summary>
        /// <returns></returns>
          [Column("FULLNAME")]
            public string FullName { get; set; }
        /// <summary>
        /// 客户简称
        /// </summary>
        /// <returns></returns>
         [Column("SHORTNAME")]
            public string ShortName { get; set; }
        /// <summary>
        /// 客户行业
        /// </summary>
        /// <returns></returns>
        [Column("CUSTINDUSTRYID")]
            public string CustIndustryId { get; set; }
        /// <summary>
        /// 客户类型
        /// </summary>
        /// <returns></returns>
         [Column("CUSTTYPEID")]
            public string CustTypeId { get; set; }
        /// <summary>
        /// 客户级别
        /// </summary>
        /// <returns></returns>
          [Column("CUSTLEVELID")]
            public string CustLevelId { get; set; }
        /// <summary>
        /// 客户程度
        /// </summary>
        /// <returns></returns>
         [Column("CUSTDEGREEID")]
            public string CustDegreeId { get; set; }
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
         [Column("CONTACT")]
            public string Contact { get; set; }
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
        /// 法人代表
        /// </summary>
        /// <returns></returns>
         [Column("LEGALPERSON")]
            public string LegalPerson { get; set; }
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
         [Column("createusername")]
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
            this.CustomerId = Guid.NewGuid().ToString();
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
            this.CustomerId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}