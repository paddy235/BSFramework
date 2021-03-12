using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：系统按钮
    /// </summary>
    [Table("BASE_MODULEBUTTON")]
    public class ModuleButtonEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 按钮主键
        /// </summary>		
        [Column("MODULEBUTTONID")]
        public string ModuleButtonId { get; set; }
        /// <summary>
        /// 功能主键
        /// </summary>		
        [Column("MODULEID")]
        public string ModuleId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>	
        [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 图标
        /// </summary>		
        [Column("ICON")]
        public string Icon { get; set; }
        /// <summary>
        /// 编码
        /// </summary>		
        [Column("ENCODE")]
        public string EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>		
        [Column("FULLNAME")]
        public string FullName { get; set; }
        /// <summary>
        /// Action地址
        /// </summary>		
        [Column("ACTIONADDRESS")]
        public string ActionAddress { get; set; }
        /// <summary>
        /// js函数名称
        /// </summary>		
        [Column("ACTIONNAME")]
        public string ActionName{ get; set; }
        /// <summary>
        /// 按钮图标
        /// </summary>		
        [Column("FAIMAGE")]
        public string FaImage { get; set; }
        /// <summary>
        /// 按钮图标
        /// </summary>		
        [Column("BUTTONTYPE")]
        public int? ButtonType { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ModuleButtonId = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ModuleButtonId = keyValue;
        }
        #endregion
    }
}