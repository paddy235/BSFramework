using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// 描 述：班组安全日活动
    /// </summary>
    [Table("WG_JOBTEMPLATE")]
    public class JobTemplateEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("JOBID")]
        public string JobId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("JOBCONTENT")]
        public string JobContent { get; set; }
        [Column("DEVICE")]
        public string Device { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("JOBSTARTTIME")]
        public DateTime? JobStartTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("JOBENDTIME")]
        public DateTime? JobEndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("JOBPERSON")]
        public string JobPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("DANGEROUS")]
        public string Dangerous { get; set; }
        /// <summary>
        /// 
        /// </summary
        [Column("MEASURE")]
        public string Measure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("DANGERTYPE")]
        public string DangerType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 周期
        /// </summary>
        [Column("CYCLE")]
        public string Cycle { get; set; }
        [Column("CYCLEDATE")]
        public string CycleDate { get; set; }
        [Column("ENABLETRAINING")]
        public bool EnableTraining { get; set; }

        [Column("JOBPERSONID")]
        public String JobPersonId { get; set; }
        [Column("OTHERPERSONID")]
        public String otherpersonid { get; set; }
        [Column("OTHERPERSON")]
        public String otherperson { get; set; }
        [Column("WORKTYPE")]
        public String WorkType { get; set; }
        [Column("WORKQUARTERS")]
        public String WorkQuarters { get; set; }
        [Column("WORKDESCRIBE")]
        public String WorkDescribe { get; set; }
        [Column("RESPREPARE")]
        public String ResPrepare { get; set; }
        [Column("WORKAREA")]
        public String WorkArea { get; set; }
        [Column("PICNUMBER")]
        public String PicNumber { get; set; }
        [Column("REDACTIONPERSON")]
        public String RedactionPerson { get; set; }
        [Column("REDACTIONDATE")]
        public DateTime? RedactionDate { get; set; }
        [Column("USETIME")]
        public Int32 Usetime { get; set; }

        [Column("CREATEUSERID")]
        public String CreateUserId { get; set; }

        [Column("CREATEUSER")]
        public String CreateUser { get; set; }

        [Column("JOBPLANTYPE")]
        public String jobplantype { get; set; }

        [Column("JOBPLANTYPEID")]
        public String jobplantypeid { get; set; }
        [Column("JOBSTANDARD")]
        public String jobstandard { get; set; }
        [Column("ISWEEK")]
        public bool isweek { get; set; }
        [Column("ISEXPLAIN")]
        public bool isexplain { get; set; }
        [Column("ISEND")]
        public bool isend { get; set; }
        [Column("ISLASTDAY")]
        public bool islastday { get; set; }
        [Column("ISWORKGROUP")]
        public bool isworkgroup { get; set; }
        /// <summary>
        /// 区分危险预知训练库与任务库
        /// </summary>
        [Column("JOBTYPE")]
        public String JobType { get; set; }
        [Column("WORKSETNAME")]
        public String worksetname { get; set; }
        [Column("WORKSETID")]
        public String worksetid { get; set; }

        [Column("EDITTIME")]
        public int EditTime { get; set; }

        [Column("PERCENT")]
        public decimal Percent { get; set; }

        /// <summary>
        /// 关联ID
        /// </summary>
        [Column("RECID")]
        public string RecId { get; set; }

        /// <summary>
        /// 检查表名称
        /// </summary>
        [Column("INSPECTIONNAME")]
        public string InspectionName { get; set; }


        /// <summary>
        /// 任务类型
        /// </summary>
        [Column("TASKTYPE")]
        public string TaskType { get; set; }

        /// <summary>
        /// 风险等级
        /// </summary>
        [Column("RISKLEVEL")]
        public string RiskLevel { get; set; }

        [NotMapped]
        public String d { get; set; }
        [NotMapped]
        public String m { get; set; }

        [NotMapped]
        public IList<DangerTemplateEntity> Dangers { get; set; }
        [NotMapped]
        public List<JobDangerousEntity> DangerousList { get; set; }

        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }

        /// <summary>
        /// 所属部门code  区分电厂数据
        /// </summary>
        [Column("DEPTCODE")]
        public String DeptCode { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.JobId = Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.JobId = keyValue;
        }
        #endregion
    }
}