using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.OndutyManage
{
    /// <summary>
    /// 人脸考勤打卡签到的时间
    /// </summary>
    [Table("wg_faceattendancetime")]
    public class FaceAttendanceTimeEntity : BaseEntity
    {

        [Column("id")]
        public String id { get; set; }


        [Column("userid")]
        public String userid { get; set; }

        //[Column("username")]
        //public String username { get; set; }

        [Column("worktime")]
        public DateTime worktime { get; set; }
    }
}
