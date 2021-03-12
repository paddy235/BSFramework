using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.OndutyManage
{
    /// <summary>
    /// 集中培训签到开关
    /// </summary>
    [Table("wg_ondutymeet")]
    public  class OndutyMeetOnoffEntity
    {
        [Column("id")]
        public String id { get; set; }
        [Column("meetid")]
        public String meetid { get; set; }

        [Column("onoff")]
        public bool onoff { get; set; }
    }
}
