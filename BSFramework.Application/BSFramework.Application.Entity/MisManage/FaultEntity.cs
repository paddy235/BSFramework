using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.MisManage
{
    public class FaultEntity
    {
        /// <summary>
        /// SBFAULTID
        /// </summary>
        public decimal FaultId { get; set; }
        /// <summary>
        /// FAULTNUM
        /// </summary>
        public string FaultNum { get; set; }
        /// <summary>
        /// SSJZ
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// ZY
        /// </summary>
        public string Specialty { get; set; }
        /// <summary>
        /// QXFL
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// DESCRIPTION
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// FXDATE
        /// </summary>
        public DateTime FoundTime { get; set; }
        /// <summary>
        /// QXMC
        /// </summary>
        public string FaultName { get; set; }
        /// <summary>
        /// XQBM
        /// </summary>
        public string ResolveGroup { get; set; }
        /// <summary>
        /// YQXQWCDATE
        /// </summary>
        public DateTime? PlanCompleteTime { get; set; }
        /// <summary>
        /// FAULTSTATUS
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// XQJSX
        /// </summary>
        public string TimeStatus { get; set; }
        /// <summary>
        /// 分配
        /// </summary>
        public bool Allocated { get; set; }
        public string ResponsibleDepartment { get; set; }
        public string FoundPerson { get; set; }
        public string FoundDepartment { get; set; }
        public string Acceptor { get; set; }
        public string AcceptanceDepartment { get; set; }
        public DateTime? AcceptanceTime { get; set; }
        public string ResolveInTime { get; set; }
        public string Qualified { get; set; }
        public string UndoReason { get; set; }
        /// <summary>
        /// 风险分析
        /// </summary>
        public string RiskAnalysis { get; set; }
        /// <summary>
        /// 风险评估
        /// </summary>
        public string RiskAssessment { get; set; }
        /// <summary>
        /// 防范措施
        /// </summary>
        public string Preventive { get; set; }
    }

    public class FaultRelationEntity
    {
        public string RelationId { get; set; }
        public string MeetingJobId { get; set; }
        public decimal FaultId { get; set; }
    }
}
