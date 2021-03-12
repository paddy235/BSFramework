using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// √Ë  ˆ£∫Œ£œ’“ÚÀÿ
    /// </summary>
    [Table("wg_dangertemplate")]
    public class DangerTemplateEntity : BaseEntity
    {

        [Column("dangerid")]
        public string DangerId { get; set; }
        [Column("jobid")]
        public string JobId { get; set; }
        [Column("dangerous")]
        public string Dangerous { get; set; }
        [Column("measure")]
        public string Measure { get; set; }
        [Column("CreateUserId")]
        public string CreateUserId { get; set; }
        [Column("createtime")]
        public DateTime CreateTime { get; set; }

        [Column("DeptCode")]
        public string DeptCode { get; set; }

        
    }
}