using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("wg_eduinventory")]
    public class EduInventoryEntity
    {

        /// <summary>
        /// 所属部门code  区分电厂数据
        /// </summary>
        [Column("DeptCode")]
        public String DeptCode { get; set; }
        [Column("ID")]
        public String ID { get; set; }
        [Column("BZID")]
        public String BZID { get; set; }
        [NotMapped]
        public String Owner { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUserName")]
        public String CreateUserName { get; set; }
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }
        [Column("ModifyDate")]
        public DateTime ModifyDate { get; set; }

        [Column("ModifyUserName")]
        public String ModifyUserName { get; set; }
        [Column("ModifyUserId")]
        public String ModifyUserId { get; set; }

        [Column("Name")]
        public String Name { get; set; }

        [Column("Question")]
        public String Question { get; set; }
        [Column("Answer")]
        public String Answer { get; set; }
        [Column("Danger")]
        public String Danger { get; set; }
        [Column("Measure")]
        public String Measure { get; set; }
        [Column("EduType")]
        public String EduType { get; set; }

        [Column("UseDeptId")]
        public String UseDeptId { get; set; }

        [Column("UseDeptCode")]
        public String UseDeptCode { get; set; }

        [Column("UseDeptName")]
        public String UseDeptName { get; set; }
        [NotMapped]
        public String kjname { get; set; }
        [NotMapped]
        public String kjpath { get; set; }
        [NotMapped]
        public String kjid { get; set; }
        [NotMapped]
        public String fm { get; set; }

        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
    }
}
