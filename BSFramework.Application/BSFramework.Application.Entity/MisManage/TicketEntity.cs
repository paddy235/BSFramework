using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.MisManage
{
    public class TicketEntity
    {
        public string TicketId { get; set; }
        /// <summary>
        /// 工作票编号
        /// </summary>
        public string TicketCode { get; set; }
        /// <summary>
        /// 机组
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 工作票种类
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 工作负责人
        /// </summary>
        public string DutyPerson { get; set; }
        /// <summary>
        /// 变更的工作负责人
        /// </summary>
        public string DutyPerson2 { get; set; }
        /// <summary>
        /// 许可工作开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }
        /// <summary>
        /// 工作内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 工作许可人
        /// </summary>
        public string ApprovePerson { get; set; }
        /// <summary>
        /// 批准工作结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 延期
        /// </summary>
        public DateTime? EndTime2 { get; set; }
        /// <summary>
        /// 工作票状态
        /// </summary>
        public string TicketStatus { get; set; }
        /// <summary>
        /// 工作地点
        /// </summary>
        public string Place { get; set; }
        public Dictionary<string, int> OtherTickets { get; set; }
        ///// <summary>
        ///// 一级动火
        ///// </summary>
        //public int Num1 { get; set; }
        ///// <summary>
        ///// 二级动火
        ///// </summary>
        //public int Num2 { get; set; }
        ///// <summary>
        ///// 风险作业审批单
        ///// </summary>
        //public int Num3 { get; set; }
        ///// <summary>
        ///// 热控保护措施票
        ///// </summary>
        //public int Num4 { get; set; }
        ///// <summary>
        ///// 继电保护措施票
        ///// </summary>
        //public int Num5 { get; set; }
        ///// <summary>
        ///// 作业安全措施票
        ///// </summary>
        public int Num6 { get; set; }
        public string DeptName { get; set; }
        public string WorkMate { get; set; }
        public string ApprovePerson2 { get; set; }
    }

    public class TicketRelationEntity
    {
        public string RelationId { get; set; }
        public string MeetingJobId { get; set; }
        public decimal FaultId { get; set; }
    }
}
