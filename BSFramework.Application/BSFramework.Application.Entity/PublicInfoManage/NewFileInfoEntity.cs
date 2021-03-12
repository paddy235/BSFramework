using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace BSFramework.Application.Entity.PublicInfoManage
{
    public class NewFileInfoEntity : BaseEntity
    {
        [Column("ID")]
        public string ID { get; set; }
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("CreateUser")]
        public string CreateUser { get; set; }
        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("RecId")]
        public string RecId { get; set; }
        [Column("Title")]
        public string Title { get; set; }
        [Column("Instruction")]
        public string Instruction { get; set; }
        [Column("Amount")]
        public int Amount { get; set; }
        [Column("IsImg")]
        public bool IsImg { get; set; }
    }
}
