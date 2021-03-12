using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PushInfoManage
{
    [Table("wg_pushperson")]
    public  class PushPersonEntity
    {
        [Column("id")]
        public string id { get; set; }
        [Column("pushid")]
        public string pushid { get; set; }
        [Column("person")]
        public string person { get; set; }
        [Column("personid")]
        public string personid { get; set; }
        [Column("isread")]
        public bool isread { get; set; }
    }
}
