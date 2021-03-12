using System;
using BSFramework.Application.Code;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：黑名单管理
    /// </summary>
    [Table("BASE_DATASET")]
    public class DataSetEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 角色主键
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
         
        /// <summary>
        /// 项目编码
        /// </summary>		
        [Column("ITEMCODE")]
        public string ItemCode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>		
        [Column("ITEMNAME")]
        public string ItemName { get; set; }
        /// <summary>
        /// 所属单位Id（多个用引文逗号分隔）
        /// </summary>		
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 所属单位编码（多个用引文逗号分隔）
        /// </summary>		
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 所属单位名称（多个用引文逗号分隔）
        /// </summary>		
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        ///是否默认项（是，否）
        /// </summary>		
        [Column("ISDEFAULT")]
        public string IsDefault { get; set; }
        /// <summary>
        ///是否启用（是，否）
        /// </summary>		
        [Column("ISOPEN")]
        public int IsOpen { get; set; }

        /// <summary>
        ///项目分类（实时监控，待办事项）
        /// </summary>		
        [Column("ITEMKIND")]
        public string ItemKind { get; set; }
        /// <summary>
        ///项目类型
        /// </summary>		
        [Column("ITEMTYPE")]
        public string ItemType { get; set; }
        /// <summary>
        ///适用角色（公司领导，一般用户）
        /// </summary>		
        [Column("ITEMROLE")]
        public string ItemRole { get; set; }

        /// <summary>
        ///显示图标
        /// </summary>		
        [Column("ICON")]
        public string Icon { get; set; }
        /// <summary>
        ///页面地址
        /// </summary>		
        [Column("ADDRESS")]
        public string Address { get; set; }
        /// <summary>
        ///执行JS函数
        /// </summary>		
        [Column("CALLBACK")]
        public string Callback { get; set; }
        /// <summary>
        ///显示样式设置
        /// </summary>		
        [Column("ITEMSTYLE")]
        public string ItemStyle { get; set; }

        /// <summary>
        ///排序号
        /// </summary>		
        [Column("SORTCODE")]
        public int SortCode { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>		
        [Column("REMARK")]
        public string Remark { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 最后一次修改时间
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToString() : Id;
            this.DeptCode = OperatorProvider.Provider.Current().DeptCode;
            this.DeptId = OperatorProvider.Provider.Current().DeptId;
            this.DeptName = OperatorProvider.Provider.Current().DeptName;
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.ModifyDate = DateTime.Now;
        }
        #endregion
    }
}