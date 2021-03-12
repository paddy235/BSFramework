using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    public class DangerEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Id { get; set; }
        /// <summary>
        /// 工作票号
        /// </summary>
        /// <returns></returns>
        public string TicketId { get; set; }
        /// <summary>
        /// 记录编号
        /// </summary>
        /// <returns></returns>
        public string Sno { get; set; }
        /// <summary>
        /// 关联工作任务id 
        /// Meetingandjobid
        /// </summary>
        /// <returns></returns>
        public string JobId { get; set; }
        /// <summary>
        /// 工作任务名称
        /// </summary>
        /// <returns></returns>
        public string JobName { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        /// <returns></returns>
        public string JobUser { get; set; }
        /// <summary>
        /// 作业地点
        /// </summary>
        /// <returns></returns>
        public string JobAddress { get; set; }
        /// <summary>
        /// 作业时间
        /// </summary>
        /// <returns></returns>
        public DateTime? JobTime { get; set; }
        /// <summary>
        /// 参与人员
        /// </summary>
        /// <returns></returns>
        public string Persons { get; set; }
        /// <summary>
        /// 措施及责任人变更情况
        /// </summary>
        /// <returns></returns>
        public string Measure { get; set; }
        /// <summary>
        /// 工作中断后措施及责任人变更情况
        /// </summary>
        /// <returns></returns>
        public string StopMeasure { get; set; }
        /// <summary>
        /// 班组Id
        /// </summary>
        /// <returns></returns>
        public string GroupId { get; set; }
        /// <summary>
        /// 班组名称
        /// </summary>
        /// <returns></returns>
        public string GroupName { get; set; }

        /// <summary>
        /// 状态（0：未结束，1：进行中，2：结束）
        /// </summary>
        /// <returns></returns>
        public int Status { get; set; }
        /// <summary>
        /// 备用字段
        /// </summary>
        /// <returns></returns>
        public string Remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string OperUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string DeptCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DateTime? OperDate { get; set; }

        [Column("IsChanged")]
        public string IsChanged { get; set; }

        [Column("AppraiseContent")]
        public String AppraiseContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<MeasuresEntity> TrainingItems { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public List<JobUserEntity> JobUsers { get; set; }

        [NotMapped]
        public List<ActivityEvaluateEntity> Evaluateions { get; set; }
        public float Score { get; set; }
        public string ScoreRemark { get; set; }

        [NotMapped]
        public string olddanger { get; set; }
        [NotMapped]
        public string oldmeasure { get; set; }
        [NotMapped]
        public string newdanger { get; set; }
        [NotMapped]
        public string newmeasure { get; set; }
        [NotMapped]
        public string Photo { get; set; }
        [NotMapped]
        public string StatusDescription { get; set; }
        [NotMapped]
        public string TemplateId { get; set; }
        [NotMapped]
        public bool? FromJob { get; set; }
        [NotMapped]
        public bool? IsEvaluate { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = string.IsNullOrEmpty(Id) ? Guid.NewGuid().ToString() : Id;
            this.DeptCode = OperatorProvider.Provider.Current().DeptCode;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.GroupName = OperatorProvider.Provider.Current().DeptName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.OperDate = DateTime.Now;
            this.OperUserId = OperatorProvider.Provider.Current().UserId;
        }
        #endregion
    }
}
