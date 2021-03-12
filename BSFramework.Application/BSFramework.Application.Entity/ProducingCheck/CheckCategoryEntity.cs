using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.ProducingCheck
{
    /// <summary>
    /// 问题类别
    /// </summary>
    public class CheckCategoryEntity
    {
        [Column("CATEGORYID")]
        public string CategoryId { get; set; }
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        [Column("PARENTID")]
        public string ParentId { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }
        [NotMapped]
        public CheckCategoryEntity ParentCategory { get; set; }
    }
}
