using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    [Table("wg_supplypeople")]
    public class SupplyPeopleEntity : BaseEntity
    {
        [Column("SupplyId")]
        public string SupplyId { get; set; }
        [Column("ID")]
        public string ID { get; set; }
        [Column("UserId")]
        public string UserId { get; set; }
        [Column("UserName")]
        public string UserName { get; set; }
    }
}
