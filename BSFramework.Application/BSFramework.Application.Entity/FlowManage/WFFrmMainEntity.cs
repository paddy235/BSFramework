using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 版 本
    /// 描 述：表单模板表
    /// </summary>
    [Table("WF_FRMMAIN")]
    public class WFFrmMainEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 表单主键
        /// </summary>		
        [Column("FRMMAINID")]
        public string FrmMainId { get; set; }
        /// <summary>
        /// 表单编码
        /// </summary>		
        [Column("FRMCODE")]
        public string FrmCode { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>		
        [Column("FRMNAME")]
        public string FrmName { get; set; }
        /// <summary>
        /// 表单类型
        /// </summary>		
        [Column("FRMTYPE")]
        public string FrmType { get; set; }
        /// <summary>
        /// 数据库Id
        /// </summary>
        [Column("FRMDBID")]
        public string FrmDbId { get; set; }
        /// <summary>
        /// 关联数据库表
        /// </summary>		
        [Column("FRMTABLE")]
        public string FrmTable { get; set; }
        /// <summary>
        /// 关联表主键
        /// </summary>
        [Column("FRMTABLEID")]
        public string FrmTableId { get; set; }
        /// <summary>
        /// 表单内容
        /// </summary>		
        [Column("FRMCONTENT")]
        public string FrmContent { get; set; }
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
        /// <summary>
        /// 是否建表，0不建表，1建表
        /// </summary>
        [Column("ISSYSTEMTABLE")]
        public int? isSystemTable { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.FrmMainId = Guid.NewGuid().ToString();
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
            this.FrmMainId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
