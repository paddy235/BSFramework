
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 版 本
    /// 描 述：工作流实例表
    /// </summary>
    [Table("WF_PROCESSINSTANCE")]
    public class WFProcessInstanceEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键
        /// </summary>		
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 实例名称
        /// </summary>
        [Column("CODE")]
        public string Code { get; set; }
        /// <summary>
        /// 自定定义标题
        /// </summary>
        [Column("CUSTOMNAME")]
        public string CustomName { get; set; }
        /// <summary>
        /// 当前节点ID
        /// </summary>		
        [Column("ACTIVITYID")]
        public string ActivityId { get; set; }
        /// <summary>
        /// 获取节点类型 0会签开始,1会签结束,2一般节点,开始节点,4流程运行结束
        /// </summary>
        [Column("ACTIVITYTYPE")]
        public int? ActivityType { get; set; }
        /// <summary>
        /// 当前节点名称
        /// </summary>		
        [Column("ACTIVITYNAME")]
        public string ActivityName { get; set; }
        /// <summary>
        /// 上一个节点Id
        /// </summary>
        [Column("PREVIOUSID")]
        public string PreviousId { get; set; }
        /// <summary>
        /// 流程实例模板Id
        /// </summary>		
        [Column("PROCESSSCHEMEID")]
        public string ProcessSchemeId { get; set; }
        /// <summary>
        /// 执行人
        /// </summary>
        [Column("MAKERLIST")]
        public string MakerList { get; set; }
        /// <summary>
        /// 实例类型
        /// </summary>
        [Column("SCHEMETYPE")]
        public string SchemeType { get; set; }
        /// <summary>
        /// 表单类型（0自定义，1系统)
        /// </summary>
        [Column("FRMTYPE")]
        public int? FrmType { get; set; }
        /// <summary>
        /// 有效标志（0暂停,1正常运行,3草稿）
        /// </summary>		
        [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
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
        /// 重要等级
        /// </summary>
        [Column("WFLEVEL")]
        public int? wfLevel { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 流程是否完成(0运行中,1运行结束,2被召回,3不同意,4表示被驳回)
        /// </summary>
        [Column("ISFINISH")]
        public int? isFinish { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
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
