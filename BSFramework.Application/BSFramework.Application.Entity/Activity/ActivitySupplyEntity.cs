using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{

    [Table("wg_activitysupply")]
    public class ActivitySupplyEntity : BaseEntity
    {
        [Column("ActivityId")]
        public string ActivityId { get; set; }
        [Column("ID")]
        public string ID { get; set; }
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("StartDate")]
        public DateTime StartDate { get; set; }
        [Column("EndDate")]
        public DateTime? EndDate { get; set; }

        [Column("IsOver")]
        public bool IsOver { get; set; }

        [Column("UserName")]
        public string UserName { get; set; }
    }
}
