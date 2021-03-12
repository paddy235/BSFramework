using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ExperienceManage
{
    [Table("wg_comment")]
    public class CommentEntity : BaseEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("BZID")]
        public String BZId { get; set; }

        [Column("BZNAME")]
        public String BZName { get; set; }

        [Column("CREATEUSERID")]
        public String CreateUserId { get; set; }

        [Column("CREATEUSERNAME")]
        public String CreateUserName { get; set; }


        [Column("TITLE")]
        public String Title { get; set; }

        [Column("CONTENT")]
        public String Content { get; set; }

        /// <summary>
        /// 匿名
        /// </summary>
        [Column("ANONYMITY")]
        public String Anonymity { get; set; }

        [Column("REMARK")]
        public String Remark { get; set; }

        [Column("CLICKCOUNT")]
        public Int32 ClickCount { get; set; }

        [Column("SUPPORT")]
        public Int32 Support { get; set; }

        [Column("OPPOSE")]
        public Int32 Oppose { get; set; }

        [Column("OBJECTID")]
        public String ObjectId { get; set; }

        [Column("SUPPORTUSERID")]
        public String SupportUserId { get; set; }

        [Column("SUPPORTUSERNAME")]
        public String SupportUserName { get; set; }

        [Column("OPPOSEUSERID")]
        public String OpposeUserId { get; set; }

        [Column("OPPOSEUSERNAME")]
        public String OpposeUserName { get; set; }

        [NotMapped]
        public IList Files { get; set; }

        [Column("COMMENTCOUNT")]
        public Int32 commentCount { get; set; }

        [NotMapped]
        public String isSupport { get; set; }

        [NotMapped]
        public String isOppose { get; set; }



    }
}
