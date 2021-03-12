using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.BaseManage
{
    /// <summary>
    /// 人脸
    /// </summary>
    public class UserFaceEntity
    {
        public string UserId { get; set; }
        public string FaceStream { get; set; }
        [NotMapped]
        public FileInfoEntity FaceFile { get; set; }
    }
}
