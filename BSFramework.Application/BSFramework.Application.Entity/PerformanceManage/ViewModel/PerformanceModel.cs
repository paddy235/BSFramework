using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.PerformanceManage.ViewModel
{
   public class PerformanceModel
    {
        /// <summary>
        /// 绩效的主键
        /// </summary>
        public string PerformanceId { get; set; }
        /// <summary>
        /// 月份 中文 例：一月
        /// </summary>
        public string Month { get; set; }
        /// <summary>
        /// 适用时间
        /// </summary>
        public string UseTime { get; set; }
        /// <summary>
        /// 标题 逗号隔开
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 得分 逗号隔开 
        /// </summary>
        public string Score { get; set; }   
    }
}
