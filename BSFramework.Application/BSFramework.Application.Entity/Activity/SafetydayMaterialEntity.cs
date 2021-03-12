using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 活动
    /// </summary>
    [Table("wg_safetydaymaterial")]
    public class SafetydayMaterialEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("deptid")]
        public string deptid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("deptname")]
        public string deptname { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("fileid")]
        public string fileid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("filename")]
        public string filename { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("safetydayid")]
        public string SafetydayId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("isread")]
        public bool  isread { get; set; }

    }
}
