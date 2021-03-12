using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.YearWorkPlan
{
    /// <summary>
    /// 年度工作首页
    /// </summary>
    public class YearWorkPlanEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public String id { get; set; }
        /// <summary>
        /// 任务
        /// </summary>
        [Column("plan")]
        public String plan { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        [Column("remark")]
        public String remark { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        [Column("planstart")]
        public String planstart { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        [Column("planend")]
        public String planend { get; set; }
        /// <summary>
        /// 计划结束时间
        /// </summary>
        [Column("planfinish")]
        public String planfinish { get; set; }
        /// <summary>
        /// 上次进度
        /// </summary>
        [Column("lastprogress")]
        public String lastprogress { get; set; }
        /// <summary>
        /// 当前进度
        /// </summary>
        [Column("progress")]
        public String progress { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        [Column("deptid")]
        public String deptid { get; set; }
        /// <summary>
        /// 部门name
        /// </summary>
        [Column("deptname")]
        public String deptname { get; set; }
        /// <summary>
        /// 部门code
        /// </summary>
        [Column("deptcode")]
        public String deptcode { get; set; }
        /// <summary>
        /// 书签
        /// </summary>
        [Column("bookmark")]
        public String bookmark { get; set; }
        /// <summary>
        /// 修改字段
        /// </summary>
        [Column("editstr")]
        public String editstr { get; set; }
        [Column("CREATEDATE")]
        public DateTime CREATEDATE { get; set; }
        [Column("CREATEUSERID")]
        public String CREATEUSERID { get; set; }
        [Column("CREATEUSERNAME")]
        public String CREATEUSERNAME { get; set; }
        [Column("MODIFYDATE")]
        public DateTime MODIFYDATE { get; set; }
        [Column("MODIFYUSERID")]
        public String MODIFYUSERID { get; set; }
        [Column("MODIFYUSERNAME")]
        public String MODIFYUSERNAME { get; set; }

        [NotMapped]
        public List<YearWorkPlanEntity> historyEntity { get; set; }
    }
}
