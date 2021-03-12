using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板内容表
    /// </summary>
    [Table("WF_SCHEMECONTENT")]
    public class WFSchemeContentEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键Id
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 流程内容ID
        /// </summary>		
        [Column("WFSCHEMEINFOID")]
        public string WFSchemeInfoId { get; set; }
        /// <summary>
        /// 流程内容版本
        /// </summary>
        [Column("SCHEMEVERSION")]
        public string SchemeVersion { get; set; }
        /// <summary>
        /// 流程内容
        /// </summary>
        [Column("SCHEMECONTENT")]
        public string SchemeContent { get; set; }
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
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
        }
        #endregion
    }
}
