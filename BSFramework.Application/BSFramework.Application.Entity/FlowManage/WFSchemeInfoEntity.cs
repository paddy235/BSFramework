using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板信息表
    /// </summary>
    [Table("WF_SCHEMEINFO")]
    public class WFSchemeInfoEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 流程主键
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 流程编码
        /// </summary>		
        [Column("SCHEMECODE")]
        public string SchemeCode { get; set; }
        /// <summary>
        /// 流程名称
        /// </summary>		
        [Column("SCHEMENAME")]
        public string SchemeName { get; set; }
        /// <summary>
        /// 流程类型
        /// </summary>		
        [Column("SCHEMETYPE")]
        public string SchemeType { get; set; }
        /// <summary>
        /// 流程内容版本
        /// </summary>
        [Column("SCHEMEVERSION")]
        public string SchemeVersion { get; set; }
        /// <summary>
        /// 流程模板使用者
        /// </summary>
        [Column("SCHEMECANUSER")]
        public string SchemeCanUser { get; set; }
        /// <summary>
        /// 表单类型（0自定义，1系统)
        /// </summary>
        [Column("FRMTYPE")]
        public int? FrmType { get; set; }
        /// <summary>
        /// 权限类型(0所有人,1指定成员)
        /// </summary>
        [Column("AUTHORIZETYPE")]
        public int? AuthorizeType { get; set; }
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
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}

