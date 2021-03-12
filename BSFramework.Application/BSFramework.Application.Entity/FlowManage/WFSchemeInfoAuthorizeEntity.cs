using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 描 述：工作流模板权限表
    /// </summary>
    [Table("WF_SCHEMEINFOAUTHORIZE")]
    public class WFSchemeInfoAuthorizeEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 流程模板信息Id
        /// </summary>
        [Column("SCHEMEINFOID")]
        public string SchemeInfoId { get; set; }
        /// <summary>
        /// 权限对象Id
        /// </summary>
        [Column("OBJECTID")]
        public string ObjectId { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
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
