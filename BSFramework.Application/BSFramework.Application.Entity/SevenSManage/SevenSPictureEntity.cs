using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{
    [Table("wg_sevenspicture")]
    public class SevenSPictureEntity
    {
        [Column("Id")]
        public String Id { get; set; }
        [Column("deptid")]
        public String deptid { get; set; }
        [Column("deptname")]
        public String deptname { get; set; }
        [Column("state")]
        public String state { get; set; }
        [Column("CreateDate")]
        public DateTime? CreateDate { get; set; }
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }
        [Column("CreateUserName")]
        public String CreateUserName { get; set; }
        [Column("ModifyDate")]
        public DateTime? ModifyDate { get; set; }
        [Column("ModifyUserId")]
        public String ModifyUserId { get; set; }
        [Column("ModifyUserName")]
        public String ModifyUserName { get; set; }
        [Column("evaluation")]
        public String evaluation { get; set; }

        [Column("evaluationUser")]
        public String evaluationUser { get; set; }
        [Column("evaluationDate")]
        public String evaluationDate { get; set; }
        [Column("planeStartDate")]
        public DateTime? planeStartDate { get; set; }
        [Column("planeEndDate")]
        public DateTime? planeEndDate { get; set; }
        [NotMapped]
        public string planeTime
        {
            get;
            set;

        }
        [NotMapped]
        public string evaluationState
        {
            get;
            set;
        }
        [NotMapped]
        public List<FileInfoEntity> Files
        {
            get;
            set;
        }

    }
}
