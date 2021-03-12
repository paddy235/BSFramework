using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity
{
    /// <summary>
    /// 描 述：注册账户
    /// </summary>
    public class AccountEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ACCOUNTID")]
        public string AccountId { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
         [Column("MOBILECODE")]
        public string MobileCode { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        [Column("SECURITYCODE")]
        public string SecurityCode { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
         [Column("PASSWORD")]
        public string Password { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
          [Column("COMPANYNAME")]
        public string CompanyName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Column("FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        [Column("REGISTERTIME")]
        public DateTime? RegisterTime { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        [Column("EXPIRETIME")]
        public DateTime? ExpireTime { get; set; }
        /// <summary>
        /// IPAddress
        /// </summary>	
         [Column("IPADDRESS")]
        public string IPAddress { get; set; }
        /// <summary>
        /// IPAddressName
        /// </summary>		
        [Column("IPADDRESSNAME")]
        public string IPAddressName { get; set; }
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
        /// 最后登录时间
        /// </summary>		
         [Column("LASTVISIT")]
        public DateTime? LastVisit { get; set; }
        /// <summary>
        /// 登录次数
        /// </summary>		
        [Column("LOGONCOUNT")]
        public int? LogOnCount { get; set; }
        /// <summary>
        /// 授权登录次数
        /// </summary>
         [Column("AMOUNTCOUNT")]
        public int? AmountCount { get; set; }
    }
}
