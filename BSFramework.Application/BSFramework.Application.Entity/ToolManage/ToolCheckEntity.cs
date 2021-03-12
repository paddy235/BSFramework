using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ToolManage
{
    [Table("wg_toolcheck")]
    public class ToolCheckEntity : BaseEntity
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


        [Column("ToolId")]
        public String ToolId { get; set; }

        [Column("CheckPeople")]
        public String CheckPeople { get; set; }

        [Column("CheckPeopleId")]
        public String CheckPeopleId { get; set; }
        [Column("CheckResult")]
        public String CheckResult { get; set; }

        [Column("Numbers")]
        public String Numbers { get; set; }

        [Column("ValiDate")]
        public DateTime ValiDate { get; set; }
        [Column("CheckDate")]
        public DateTime CheckDate { get; set; }

        [NotMapped]
        public string CheckPG { get; set; }
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Spec { get; set; }
        [NotMapped]
        public string Path { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
