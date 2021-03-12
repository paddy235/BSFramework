using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 描 述：系统功能
    /// </summary>
    /// BASE_MODULE
    [Table("BASE_MODULECOLUMN")]
    public class ModuleEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 功能主键
        /// </summary>
        [Column("MODULEID")]
        public string ModuleId { set; get; }
        /// <summary>
        /// 父级主键
        /// </summary>
        [Column("PARENTID")]
        public string ParentId { set; get; }
        /// <summary>
        /// 编号
        /// </summary>
        [Column("ENCODE")]
        public string EnCode { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
        [Column("FULLNAME")]
        public string FullName { set; get; }
        /// <summary>
        /// 图标
        /// </summary>
        [Column("ICON")]
        public string Icon { set; get; }
        /// <summary>
        /// 导航地址
        /// </summary>
        [Column("URLADDRESS")]
        public string UrlAddress { set; get; }
        /// <summary>
        /// 导航目标
        /// </summary>
        [Column("TARGET")]
        public string Target { set; get; }
        /// <summary>
        /// 菜单选项
        /// </summary>
        [Column("ISMENU")]
        public int? IsMenu { set; get; }
        /// <summary>
        /// 允许展开
        /// </summary>
        [Column("ALLOWEXPAND")]
        public int? AllowExpand { set; get; }
        /// <summary>
        /// 是否公开
        /// </summary>
        [Column("ISPUBLIC")]
        public int? IsPublic { set; get; }
        /// <summary>
        /// 允许编辑
        /// </summary>
        [Column("ALLOWEDIT")]
        public int? AllowEdit { set; get; }
        /// <summary>
        /// 允许删除
        /// </summary>
        [Column("ALLOWDELETE")]
        public int? AllowDelete { set; get; }
        /// <summary>
        /// 排序码
        /// </summary>
        [Column("SORTCODE")]
        public int? SortCode { set; get; }
        /// <summary>
        /// 删除标记
        /// </summary>
        [Column("DELETEMARK")]
        public int? DeleteMark { set; get; }
        /// <summary>
        /// 有效标志
        /// </summary>
        [Column("ENABLEDMARK")]
        public int? EnabledMark { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column("DESCRIPTION")]
        public string Description { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime? CreateDate { set; get; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        [Column("CREATEUSERID")]
        public string CreateUserId { set; get; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { set; get; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { set; get; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        [Column("MODIFYUSERID")]
        public string ModifyUserId { set; get; }
        /// <summary>
        /// 修改用户
        /// </summary>

        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { set; get; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ModuleId = Guid.NewGuid().ToString();
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
            this.ModuleId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
