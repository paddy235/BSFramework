using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 描 述：系统视图
    /// </summary>
    [Table("BASE_MODULECOLUMN")]
    public class ModuleColumnEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 列主键
        /// </summary>	
        [Column("MODULECOLUMNID")]
        public string ModuleColumnId { get; set; }
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
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ModuleColumnId = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ModuleColumnId = keyValue;
        }
        #endregion
    }
}
