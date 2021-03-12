using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 评价流程设置
    /// </summary>
    [Table("wg_evaluatetodo")]
  public  class EvaluateTodoEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("Id")]
        public string Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column("activitydeptid")]
        public string activitydeptid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("userid")]
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("stepsid")]
        public string stepsid { get; set; }
        /// <summary>
        /// 模块
        /// </summary>
        [Column("module")]
        public string module { get; set; }

        /// <summary>
        /// 模块
        /// </summary>
        [Column("activityid")]
        public string activityid { get; set; }
    }
}
