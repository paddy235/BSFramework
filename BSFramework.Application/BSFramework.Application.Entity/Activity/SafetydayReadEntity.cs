using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 活动
    /// </summary>
    [Table("wg_SafetydayRead")]
    public class SafetydayReadEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("safetydayreadid")]
        public string SafetydayReadId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("deptid")]
        public string Deptid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("userid")]
        public string Userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("safetydayid")]
        public string SafetydayId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("isread")]
        public int? IsRead { get; set; }
                /// <summary>
        /// 
        /// </summary>
        [Column("activitytype")]
        public string activitytype { get; set; }
        

    }
}
