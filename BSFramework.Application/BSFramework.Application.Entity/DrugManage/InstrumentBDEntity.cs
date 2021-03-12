using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{

    [Table("wg_instrumentbd")]
    public class InstrumentBDEntity : BaseEntity
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
        [Column("InstrumentId")]
        public String InstrumentId { get; set; }
        [Column("InstrumentName")]
        public String InstrumentName { get; set; }
        [Column("BDDate")]
        public DateTime BDDate { get; set; }
        [Column("BeforeBDState")]
        public String BeforeBDState { get; set; }
        [Column("BDDept")]
        public String BDDept { get; set; }
        [Column("BDDeptId")]
        public String BDDeptId { get; set; }
        [Column("BDPeople")]
        public String BDPeople { get; set; }
        [Column("BDPeopleId")]
        public String BDPeopleId { get; set; }
        [Column("BDResult")]
        public String BDResult { get; set; }
        [Column("BDRemind")]
        public DateTime BDRemind { get; set; }

        [Column("BDValidate")]
        public DateTime? BDValidate { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }

        [NotMapped]
        public String Path { get; set; }
    }
}
