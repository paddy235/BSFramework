using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DeptCycleTaskManage
{
    /// <summary>
    /// 部门定期任务库
    /// </summary>
    [Table("wg_deptcycletask")]
    public class DeptCycleTaskEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createtime { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public string createuserid { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime modifytime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string createuser { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        public string modifyuser { get; set; }
        /// <summary>
        /// 修改人id
        /// </summary>
        public string modifyuserid { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string content { get; set; }
        /// <summary>
        /// 周期 每天每月每年
        /// </summary>
        public string cycle { get; set; }
        /// <summary>
        /// 周期数据
        /// </summary>
        public string cycledate { get; set; }
        /// <summary>
        /// 是否双休
        /// </summary>
        public bool isweek { get; set; }
        /// <summary>
        /// 是否最后一天
        /// </summary>
        public bool islastday { get; set; }
        /// <summary>
        /// 是否截止
        /// </summary>
        public bool isend { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string deptname { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public string deptid { get; set; }
        /// <summary>
        /// 部门code
        /// </summary>
        public string deptcode { get; set; }
        /// <summary>
        /// 责任人
        /// </summary>
        public string dutyuser { get; set; }
        /// <summary>
        /// 责任人id
        /// </summary>
        public string dutyuserid { get; set; }
        /// <summary>
        /// 周期拼接
        /// </summary>
        [NotMapped]
        public string cycleDataStr { get; set; }
        /// <summary>
        /// 开始时间  用于推送任务
        /// </summary>
        [NotMapped]
        public DateTime? starttime { get; set; }
        /// <summary>
        /// 结束时间用于推送任务
        /// </summary>
        [NotMapped]
        public DateTime? endtime { get; set; }

    }
}
