using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 评价
    /// </summary>
    [Table("wg_activityevaluate")]
    public class ActivityEvaluateEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("activityevaluateid")]
        public string ActivityEvaluateId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("activityid")]
        public string Activityid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("score")]
        public decimal Score { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("evaluatecontent")]
        public string EvaluateContent { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("evaluateuser")]
        public string EvaluateUser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("evaluateid")]
        public string EvaluateId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("evaluatedate")]
        public DateTime EvaluateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("nature")]
        public string Nature { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CREATEDATE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERID")]
        public string CREATEUSERID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEUSERNAME")]
        public string CREATEUSERNAME { get; set; }
        public string DeptName { get; set; }
        public string EvaluateDeptId { get; set; }

    }

    public class ToEvaluateEntity
    {
        public string ToEvaluateId { get; set; }
        public string BusinessId { get; set; }
        public string EvaluateDeptId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDone { get; set; }
    }
}
