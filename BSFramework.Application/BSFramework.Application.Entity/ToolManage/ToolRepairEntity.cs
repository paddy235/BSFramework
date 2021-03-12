using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("wg_toolrepair")]
    public class ToolRepairEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("DEPTID")]
        public String DeptId { get; set; }

        [Column("BZID")]
        public String BZId { get; set; }
        [Column("CREATEUSERID")]
        public String CreateUserId { get; set; }

        [NotMapped]
        public string Path { get; set; }

        [Column("ToolId")]
        public String ToolId { get; set; }

        [Column("RepairPeople")]
        public String RepairPeople { get; set; }

        [Column("RepairPeopleId")]
        public String RepairPeopleId { get; set; }
        [Column("RepairResult")]
        public String RepairResult { get; set; }

        [Column("Numbers")]
        public String Numbers { get; set; }

        [Column("Amount")]
        public Int32 Amount { get; set; }
        [Column("RepairDate")]
        public DateTime RepairDate { get; set; }

        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Spec { get; set; }

        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
