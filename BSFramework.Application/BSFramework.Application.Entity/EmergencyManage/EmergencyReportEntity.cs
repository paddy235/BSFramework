using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EmergencyManage
{
    [Table("wg_emergencyreport")]
    public class EmergencyReportEntity : BaseEntity
    {
        public EmergencyReportEntity()
        {
            this.EmergencyPersons = new List<EmergencyPersonEntity>();
            this.File = new List<FileInfoEntity>();
        }
        [Column("emergencyreportid")]
        public String EmergencyReportId { get; set; }
        [Column("emergencyid")]
        public String EmergencyId { get; set; }
        [Column("effectreport")]
        public String effectreport { get; set; }
        [Column("planreport")]
        public String planreport { get; set; }
        [Column("mainpoints")]
        public String mainpoints { get; set; }

        [Column("evaluation")]
        public String evaluation { get; set; }
        [Column("evaluationuser")]
        public String evaluationuser { get; set; }
        [Column("evaluationdate")]
        public DateTime evaluationdate { get; set; }
        [Column("evaluationscore")]
        public decimal evaluationscore { get; set; }
        [Column("score")]
        public decimal score { get; set; }
        [Column("emergencyname")]
        public String emergencyname { get; set; }
        [Column("emergencyreportname")]
        public String emergencyreportname { get; set; }
        [Column("rehearsetype")]
        public String rehearsetype { get; set; }
        [Column("userperson")]
        public String userperson { get; set; }
        [Column("planstarttime")]
        public DateTime planstarttime { get; set; }

        [Column("chairperson")]
        public String chairperson { get; set; }
        [Column("alerttype")]
        public String alerttype { get; set; }
        [Column("emergencyplace")]
        public String emergencyplace { get; set; }
        [Column("purpose")]
        public String purpose { get; set; }
        [Column("rehearsescenario")]
        public String rehearsescenario { get; set; }
        [Column("isupdate")]
        public bool isupdate { get; set; }
        [Column("deptname")]
        public String deptname { get; set; }
        [Column("deptid")]
        public String deptid { get; set; }
        [Column("emergencytype")]
        public String emergencytype { get; set; }
        [Column("emergencytypeid")]
        public String emergencytypeid { get; set; }
        
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
        public String chairpersonid { get; set; }
        [NotMapped]
        public String rehearsetypeid { get; set; }
        [NotMapped]
        public string PersonId { get; set; }
        [NotMapped]
        public string Persons { get; set; }
        [NotMapped]
        public string emergencyplan { get; set; }
        [Column("state")]
        public string state { get; set; }

        [NotMapped]
        public IList<EmergencyPersonEntity> EmergencyPersons { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> File { get; set; }

        [NotMapped]
        public IList<EmergencyReportStepsEntity> EmergencyReportSteps { get; set; }
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.EmergencyReportId))
            {
                this.EmergencyReportId = Guid.NewGuid().ToString();
            }
            this.score = 10;
            this.evaluationscore = 10;
            this.CREATEDATE = DateTime.Now;
            this.CREATEUSERID = OperatorProvider.Provider.Current().UserId;
            this.CREATEUSERNAME = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {

            this.EmergencyReportId = keyValue;
            this.MODIFYDATE = DateTime.Now;
            this.MODIFYUSERID = OperatorProvider.Provider.Current().UserId;
            this.MODIFYUSERNAME = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
    
}
