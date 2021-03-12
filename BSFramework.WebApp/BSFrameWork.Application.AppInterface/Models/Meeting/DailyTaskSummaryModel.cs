using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models.Meeting
{
    /// <summary>
    /// 首页今日工作
    /// </summary>
    public class DailyTaskSummaryModel
    {
        /// <summary>
        /// 今日工作数
        /// </summary>
        public int TodayTotal { get; set; }
        /// <summary>
        /// 已完成工作数
        /// </summary>
        public int FinishedTotal { get; set; }
        /// <summary>
        /// 未完成工作数
        /// </summary>
        public int UnFinishedTotal { get; set; }
    }
}