using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
     [Table("wg_eduplan")]
    public class EduPlanEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }


        [Column("BZName")]
        public String BZName { get; set; }

        [Column("BZID")]
        public String BZID { get; set; }

        [Column("Year")]
        public Int32 Year { get; set; }

        [Column("SubmitDate")]
        public DateTime SubmitDate { get; set; }

        [Column("SubmitState")]
        public String SubmitState { get; set; }

        [Column("VerifyState")]
        public String VerifyState { get; set; }

        [Column("Name")]
        public String Name { get; set; }

        [NotMapped]
        public String State { get; set; }
        [NotMapped]
        public String NewMessage { get; set; }
    }
}
