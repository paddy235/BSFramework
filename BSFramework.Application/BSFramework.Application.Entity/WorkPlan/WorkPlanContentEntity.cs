using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WorkPlan
{
    [Table("wg_workplancontent")]
    public class WorkPlanContentEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        [Column("BZID")]
        public String BZID { get; set; }
        [Column("PlanId")]
        public String PlanId { get; set; }
        [Column("ParentId")]
        public String ParentId { get; set; }
        [Column("WorkContent")]
        public String WorkContent { get; set; }
        [Column("IsFinished")]
        public String IsFinished { get; set; }
        [Column("Remark")]
        public String Remark { get; set; }

        [Column("StartDate")]
        public DateTime? StartDate { get; set; }

        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("WorkPeopleId")]
        public String WorkPeopleId { get; set; }

        [Column("WorkPeopleName")]
        public String WorkPeopleName { get; set; }

        [Column("DeleteRemark")]
        public Boolean DeleteRemark { get; set; }

        [Column("ChangeRemark")]
        public String ChangeRemark { get; set; }
        [NotMapped]
        public String Start { get; set; }
        [NotMapped]
        public String End { get; set; }

        [NotMapped]
        public List<WorkPlanContentEntity> ChildrenContent { get; set; }
        [NotMapped]
        public string PlanType { get; set; }
    }
}
