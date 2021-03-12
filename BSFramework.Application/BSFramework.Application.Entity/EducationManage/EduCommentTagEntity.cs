using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    /// <summary>
    /// 点评标签表
    /// </summary>
     [Table("wg_educommenttag")]
    public class EduCommentTagEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        [Column("DeptId")]
        public String DeptId { get; set; }

        [Column("Tag")]
        public String Tag { get; set; }


    }
}
