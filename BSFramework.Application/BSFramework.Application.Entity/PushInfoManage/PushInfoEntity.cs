using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace BSFramework.Application.Entity.PushInfoManage
{
    [Table("wg_pushinfo")]
    public class PushInfoEntity : BaseEntity
    {
        #region 实体成员
        [Column("pushid")]
        public string pushid { get; set; }
        [Column("pushuser")]
        public string pushuser { get; set; }
        [Column("pushuserid")]
        public string pushuserid { get; set; }
        [Column("createdate")]
        public DateTime createdate { get; set; }
        [Column("person")]
        public string person { get; set; }
        [Column("personid")]
        public string personid { get; set; }
        [Column("content")]
        public string content { get; set; }
        [Column("detailid")]
        public string detailid { get; set; }
        [NotMapped]
        public int count { get; set; }
        [NotMapped]
        public int noreadcount { get; set; }
        [NotMapped]
        public string[] NoreadPerson { get; set; }
        [NotMapped]
        public bool isread { get; set; }
        #endregion


    }
}
