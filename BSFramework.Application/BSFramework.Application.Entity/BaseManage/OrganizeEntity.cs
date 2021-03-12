using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：机构管理
    /// </summary>
    [Table("Base_Organize")]
    public class OrganizeEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 机构主键
        /// </summary>	
        [Column("ORGANIZEID")]
        public string OrganizeId { get; set; }
        /// <summary>
        /// 机构分类
        /// </summary>		
        [Column("CATEGORY")]
        public int? Category { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>		
        [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 公司外文
        /// </summary>		
        [Column("ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 公司中文
        /// </summary>		
        [Column("SHORTNAME")]
        public string ShortName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>		
        [Column("FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// 公司性质
        /// </summary>		
        [Column("NATURE")]
        public string Nature { get; set; }
        /// <summary>
        /// 外线电话
        /// </summary>		
        [Column("OUTERPHONE")]
        public string OuterPhone { get; set; }
        /// <summary>
        /// 内线电话
        /// </summary>		
        [Column("INNERPHONE")]
        public string InnerPhone { get; set; }
        /// <summary>
        /// 传真
        /// </summary>		
        [Column("FAX")]
        public string Fax { get; set; }
        /// <summary>
        /// 邮编
        /// </summary>		
        [Column("POSTALCODE")]
        public string Postalcode { get; set; }
        /// <summary>
        /// 电子邮箱
        /// </summary>		
        [Column("EMAIL")]
        public string Email { get; set; }
        /// <summary>
        /// 负责人主键
        /// </summary>		
        [Column("MANAGERID")]
        public string ManagerId { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>		
        [Column("MANAGER")]
        public string Manager { get; set; }
        /// <summary>
        /// 省主键
        /// </summary>		
        [Column("PROVINCEID")]
        public string ProvinceId { get; set; }
        /// <summary>
        /// 市主键
        /// </summary>		
        [Column("CITYID")]
        public string CityId { get; set; }
        /// <summary>
        /// 县/区主键
        /// </summary>		
        [Column("COUNTYID")]
        public string CountyId { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>		
        [Column("ADDRESS")]
        public string Address { get; set; }
        /// <summary>
        /// 公司官方
        /// </summary>		
        [Column("WEBADDRESS")]
        public string WebAddress { get; set; }
        /// <summary>
        /// 成立时间
        /// </summary>		
        [Column("FOUNDEDTIME")]
        public DateTime? FoundedTime { get; set; }
        /// <summary>
        /// 经营范围
        /// </summary>		
        [Column("BUSINESSSCOPE")]
        public string BusinessScope { get; set; }
        /// <summary>
        /// 层
        /// </summary>		
        [Column("LAYER")]
        public int? Layer { get; set; }
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
            this.OrganizeId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.OrganizeId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}