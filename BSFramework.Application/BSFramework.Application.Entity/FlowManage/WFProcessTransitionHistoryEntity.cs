using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.FlowManage
{
    /// <summary>
    /// 描 述：工作流实例流转历史记录
    /// </summary>
    [Table("WF_PROCESSTRANSITIONHISTORY")]
    public class WFProcessTransitionHistoryEntity : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 主键Id
        /// </summary>
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 实例进程ID
        /// </summary>
        [Column("PROCESSID")]
        public string ProcessId { get; set; }
        /// <summary>
        /// 开始节点Id
        /// </summary>
        [Column("FROMNODEID")]
        public string fromNodeId { get; set; }
        /// <summary>
        /// 开始节点类型
        /// </summary>
        [Column("FROMNODETYPE")]
        public int? fromNodeType { get; set; }
        /// <summary>
        /// 开始节点名称
        /// </summary>
        [Column("FROMNODENAME")]
        public string fromNodeName { get; set; }
        /// <summary>
        /// 结束节点Id
        /// </summary>
        [Column("TONODEID")]
        public string toNodeId { get; set; }
        /// <summary>
        /// 结束节点类型
        /// </summary>
        [Column("TONODETYPE")]
        public int? toNodeType { get; set; }
        /// <summary>
        /// 结束节点名称
        /// </summary>
        [Column("TONODENAME")]
        public string toNodeName { get; set; }
        /// <summary>
        /// 转化状态0正常，1驳回
        /// </summary>
        [Column("TRANSITIONSATE")]
        public int? TransitionSate { get; set; }
        /// <summary>
        /// 是否结束
        /// </summary>
        [Column("ISFINISH")]
        public int? isFinish { get; set; }
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
