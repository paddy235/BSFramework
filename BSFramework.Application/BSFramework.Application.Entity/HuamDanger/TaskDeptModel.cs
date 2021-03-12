using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.HuamDanger
{
    /// <summary>
    /// 人身风险预控库  任务_部门Id 类
    /// </summary>
   public class TaskDeptModel
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DeptId { get; set; }
    }
}
