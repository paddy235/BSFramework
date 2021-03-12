using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.AttendManage
{
    [Table("wg_attend")]
    public class AttendEntity : BaseEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("USERNAME")]
        public String UserName { get; set; }

        [Column("USERID")]
        public String UserId { get; set; }
        [Column("BZID")]
        public String BZId { get; set; }
        [Column("BZNAME")]
        public String BZName { get; set; }

        [Column("ATTENDDATE")]
        public DateTime? AttendDate { get; set; }

        [Column("LNG")]
        public String Lng { get; set; }
        [Column("LAT")]
        public String Lat { get; set; }

        [Column("REMARK")]
        public String Remark { get; set; }


    }
}
