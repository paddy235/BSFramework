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
    /// 集中培训人脸签到
    /// </summary>
    [Table("wg_ondutymeet")]
   public  class OndutyMeetEntity : BaseEntity
    {
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("id")]
        public String id { get; set; }
        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        [Column("CreateUserName")]
        public String CreateUserName { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 签到人
        /// </summary>
        [Column("ondutyuser")]
        public String ondutyuser { get; set; }
        /// <summary>
        /// 签到
        /// </summary>
        [Column("ondutydept")]
        public String ondutydept { get; set; }
        /// <summary>
        /// 签到
        /// </summary>
        [Column("ondutydeptid")]
        public String ondutydeptid { get; set; }
        /// <summary>
        /// 签到
        /// </summary>
        [Column("ondutydeptcode")]
        public String ondutydeptcode { get; set; }

        /// <summary>
        /// 签到班次
        /// </summary>
        [Column("ondutyshift")]
        public String ondutyshift { get; set; }
        /// <summary>
        /// 签到人id
        /// </summary>
        [Column("ondutyuserid")]
        public String ondutyuserid { get; set; }
        /// <summary>
        /// 签到内容
        /// </summary>
        [Column("ondutycontext")]
        public String ondutycontext { get; set; }
        /// <summary>
        /// 签到时间
        /// </summary>
        [Column("ondutytime")]
        public DateTime ondutytime { get; set; }

        /// <summary>
        /// otherid
        /// </summary>
        [Column("otherid")]
        public String otherid { get; set; }

        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }

        [NotMapped]
        public string path { get; set; }

        [NotMapped]
        public string photo { get; set; }
    }
}
