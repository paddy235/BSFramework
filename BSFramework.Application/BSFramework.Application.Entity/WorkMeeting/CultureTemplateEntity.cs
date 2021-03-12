using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.WorkMeeting
{
    /// <summary>
    /// √Ë  ˆ£∫∞‡«∞∞‡∫Ûª·
    /// </summary>
    [Table("wg_culturetemplate")]
    public class CultureTemplateEntity : BaseEntity
    {
        public string CultureTemplateId { get; set; }
        public string CultureTemplateSubject { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUserId { get; set; }
        [NotMapped]
        public List<CultureTemplateItemEntity> Contents { get; set; }
    }

    [Table("wg_culturetemplateitem")]
    public class CultureTemplateItemEntity : BaseEntity
    {
        public string CultureTemplateItemId { get; set; }
        public string CultureTemplateId { get; set; }
        public string ContentSubject { get; set; }
        public string CultureContent { get; set; }
        public DateTime CreateTime { get; set; }
        public string CreateUserId { get; set; }
    }
}