using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.AttendManage
{
    [Table("wg_setattend")]
    public class SetAttendEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("LNG")]
        public String Lng { get; set; }
        [Column("LAT")]
        public String Lat { get; set; }

        [Column("AREA")]
        public String Area { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
    }
}
