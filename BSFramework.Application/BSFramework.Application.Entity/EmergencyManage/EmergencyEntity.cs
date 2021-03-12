using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EmergencyManage
{
    [Table("wg_emergency")]
    public class EmergencyEntity : BaseEntity
    {
        [Column("ID")]
        public String ID { get; set; }


        [Column("NAME")]
        public String Name { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("BZID")]
        public String BZId { get; set; }

        [Column("BZNAME")]
        public String BZName { get; set; }

        [Column("PATH")]
        public String Path { get; set; }

        [Column("REMARK")]
        public String Remark { get; set; }
        [Column("TYPEID")]
        public String TypeId { get; set; }

        [Column("CREATEUSERID")]
        public String CREATEUSERID { get; set; }
        [Column("CREATEUSERNAME")]
        public String CREATEUSERNAME { get; set; }
        [Column("MODIFYDATE")]
        public DateTime MODIFYDATE { get; set; }
        [Column("MODIFYUSERID")]
        public String MODIFYUSERID { get; set; }
        [Column("MODIFYUSERNAME")]
        public String MODIFYUSERNAME { get; set; }
        [Column("seenum")]
        public int seenum { get; set; }
        [NotMapped]
        public string TypeName { get; set; }
        [NotMapped]
        public string FilePath { get; set; }
        [NotMapped]
        public string FileId { get; set; }
        [NotMapped]
        public string urlFilePath { get; set; }


    }
}
