using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFrameWork.Application.AppInterface.Models
{
    public class SafetydayIEntity
    {
        public string Id { get; set; }
        public string Subject { get; set; }
        public string CreateUserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string Explain { get; set; }
        public IList<FileIEntity> Files { get; set; }
        public int? isRead { get; set; }
    }

    public class FileIEntity
    {
        public string FileId { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string FolderId { get; set; }
        public string FilePath { get; set; }
        public string RecId { get; set; }
        public string ShareLink { get; set; }
        /// <summary>
        /// 预览地址
        /// </summary>
        public string ViewUrl { get; set; }
        /// <summary>
        /// 是否可预览
        /// </summary>
        public bool CanView { get; set; }
        public int IsShare { get; set; }
    }
}