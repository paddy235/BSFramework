using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.WeChatManage
{
    /// <summary>
    /// 描 述：企业号应用自定义菜单
    /// </summary>
    [Table("WECHAT_APPMENU")]
    public class WeChatAppMenuEntity
    {
        /// <summary>
        /// 菜单主键
        /// </summary>
        [Column("MENUID")]
        public string MenuId { get; set; }
        /// <summary>
        /// 菜单标题
        /// </summary>
        [Column("MENUNAME")]
        public string MenuName { get; set; }
        /// <summary>
        /// 跳转URL
        /// </summary>
        [Column("MENUURL")]
        public string MenuUrl { get; set; }
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        [Column("MENUTYPE")]
        public string MenuType { get; set; }
        /// <summary>
        /// 菜单的响应动作类型
        /// </summary>
        [Column("MENUTYPENAME")]
        public string MenuTypeName { get; set; }
        /// <summary>
        /// 菜单等级
        /// </summary>
        [Column("LEVEL")]
        public int? Level { get; set; }
        /// <summary>
        /// 菜单节点
        /// </summary>
        [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>		
        [Column("SORTCODE")]
        public int? SortCode { get; set; }
    }
}
