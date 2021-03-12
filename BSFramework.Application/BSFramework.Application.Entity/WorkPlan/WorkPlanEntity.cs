using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkPlan
{
    [Table("wg_workplan")]
    public class WorkPlanEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }



        [Column("PlanType")]
        public String PlanType { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        [Column("UseDeptId")]
        public String UseDeptId { get; set; }

        [Column("UseDeptCode")]
        public String UseDeptCode { get; set; }

        [Column("UseDeptName")]
        public String UseDeptName { get; set; }

        [Column("IsFinished")]
        public String IsFinished { get; set; }

        [Column("Remark")]
        public String Remark { get; set; }

        [Column("DeleteRemark")]
        public Boolean DeleteRemark { get; set; }

        [NotMapped]
        public String date { get; set; }

    }
}
