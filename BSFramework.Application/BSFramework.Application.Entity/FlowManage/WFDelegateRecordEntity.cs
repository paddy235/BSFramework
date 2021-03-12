using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 描 述：工作流委托记录表
    /// </summary>
    [Table("WF_DELEGATERECORD")]
    public class WFDelegateRecordEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 委托规则Id
        /// </summary>
        [Column("WFDELEGATERULEID")]
        public string WFDelegateRuleId { get; set; }
        /// <summary>
        /// 委托人Id
        /// </summary>
        [Column("FROMUSERID")]
        public string FromUserId { get; set; }
        /// <summary>
        /// 委托人
        /// </summary>		
        [Column("FROMUSERNAME")]
        public string FromUserName { get; set; }
        /// <summary>
        /// 被委托人Id
        /// </summary>		
        [Column("TOUSERID")]
        public string ToUserId { get; set; }
        /// <summary>
        /// 被委托人名称
        /// </summary>		
        [Column("TOUSERNAME")]
        public string ToUserName { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 流程实例Id
        /// </summary>		
        [Column("PROCESSID")]
        public string ProcessId { get; set; }
        /// <summary>
        /// 实例编号
        /// </summary>		
        [Column("PROCESSCODE")]
        public string ProcessCode { get; set; }
        /// <summary>
        /// 实例自定义名称
        /// </summary>
        [Column("PROCESSNAME")]
        public string ProcessName { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;        }
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
