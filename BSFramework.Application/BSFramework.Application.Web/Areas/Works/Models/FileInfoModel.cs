using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BSFramework.Application.Web.Areas.Works.Models
{
    public class FileInfoModel
    {
        public string FileId { get; set; }
        /// <summary>
        /// 文件名  带后缀
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string FilePath { get; set; }
        /// <summary>
        /// 后缀名
        /// </summary>
        public string FileExtensions { get; set; }
        public string Description { get; set; }
    }
}