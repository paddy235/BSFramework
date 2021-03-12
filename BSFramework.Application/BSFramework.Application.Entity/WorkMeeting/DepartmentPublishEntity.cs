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
    /// 描 述：要务公开
    /// </summary>
    public class DepartmentPublishEntity : BaseEntity
    {
        public string PublishId { get; set; }
        public string Content { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string CreateUserId { get; set; }
        public DateTime CreateDate { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
    }
}