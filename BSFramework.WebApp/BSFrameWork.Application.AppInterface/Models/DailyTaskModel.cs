using System;
using System.Collections.Generic;

namespace BSFrameWork.Application.AppInterface.Models
{
    /// <summary>
    /// 工作任务
    /// </summary>
    public class DailyTaskModel
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 工作任务
        /// </summary>
        public string Job { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 完成情况
        /// </summary>
        public string IsFinished { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// 作业人
        /// </summary>
        public List<DailyTaskPersonModel> TaskPersons { get; set; }
    }

    /// <summary>
    /// 作业人
    /// </summary>
    public class DailyTaskPersonModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 人员
        /// </summary>
        public string Person { get; set; }
        /// <summary>
        /// 作业人类型
        /// </summary>
        public string Category { get; set; }
    }
}