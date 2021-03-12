using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("wg_eduplanverify")]
    public class EduPlanVerifyEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }


        [Column("VerifyDate")]
        public DateTime VerifyDate { get; set; }

        [Column("VerifyResult")]
        public String VerifyResult { get; set; }
        [Column("VerifyPerson")]
        public String VerifyPerson { get; set; }
        [Column("VerifyPersonId")]
        public String VerifyPersonId { get; set; }
        [Column("VerifyContent")]
        public String VerifyContent { get; set; }
        [Column("PlanId")]
        public String PlanId { get; set; }

        [Column("Read")]
        public String Read { get; set; }
    }
}
