using BSFramework.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkMeeting
{
    /// <summary>
    /// 安全交底
    /// </summary>

    [Table("WG_DANGERANALYSIS")]
    
    public class DangerAnalysisEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ANALYSISID")]
        public string AnalysisId { get; set; }
        /// <summary>
        /// 班前会
        /// </summary>
        [Column("MEETINGID")]
        public string MeetingId { get; set; }
        /// <summary>
        /// 班组
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 班组
        /// </summary>
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        /// 班次
        /// </summary>
        [Column("MEETINGNAME")]
        public string MeetingName { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        [Column("MEETINGTIME")]
        public string MeetingTime { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        [Column("MEETINGDATE")]
        public DateTime? MeetingDate { get; set; }
        /// <summary>
        /// 风险点
        /// </summary>
        [NotMapped]
        public List<JobDangerousEntity> Dangers { get; set; }
    }
}
