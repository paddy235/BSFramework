using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{

    [Table("wg_instrumentcheck")]
    public class InstrumentCheckEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        [Column("BZID")]
        public String BZID { get; set; }
        [Column("DeptId")]
        public String DeptId { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }
        [Column("CreateUserName")]
        public String CreateUserName { get; set; }
        [Column("InstrumentId")]
        public String InstrumentId { get; set; }
        [Column("InstrumentName")]
        public String InstrumentName { get; set; }
        [Column("CheckDate")]
        public DateTime CheckDate { get; set; }
        [Column("CheckValidate")]
        public DateTime? CheckValidate { get; set; }
        [Column("CheckDept")]
        public String CheckDept { get; set; }
        [Column("CheckDeptId")]
        public String CheckDeptId { get; set; }
        [Column("CheckPeople")]
        public String CheckPeople { get; set; }
        [Column("CheckPeopleId")]
        public String CheckPeopleId { get; set; }
        [Column("CheckResult")]
        public String CheckResult { get; set; }
        [Column("CheckRemind")]
        public DateTime? CheckRemind { get; set; }

        //[Column("Validate")]
        //public DateTime? Validate { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public String Path { get; set; }
    }
}
