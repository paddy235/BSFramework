using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.DrugManage
{
    /// <summary>
    /// 药品信息表
    /// </summary>
    [Table("wg_druginventory")]
    public class DrugInventoryEntity : BaseEntity
    {

        /// <summary>
        /// 所属部门code  区分电厂数据
        /// </summary>
        [Column("DeptCode")]
        public String DeptCode { get; set; }
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        [Column("DrugName")]
        public string DrugName { get; set; }

        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }


        [Column("englishname")]
        public string EnglishName { get; set; }
        [Column("CASNO")]
        public string CASNO { get; set; }
        [Column("molecularformula")]
        public string MolecularFormula { get; set; }
        [Column("molecularweight")]
        public string MolecularWeight { get; set; }
        [Column("properties")]
        public string Properties { get; set; }
        [Column("purpose")]
        public string Purpose { get; set; }
        [Column("dangerinstruction")]
        public string DangerInstruction { get; set; }
        [Column("measure")]
        public string Measure { get; set; }
        [Column("danger")]
        public string Danger { get; set; }
        [Column("position")]
        public string Position { get; set; }
        [Column("dispose")]
        public string Dispose { get; set; }

        [NotMapped]
        public string msds { get; set; }

        [NotMapped]
        public string video { get; set; }

        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
