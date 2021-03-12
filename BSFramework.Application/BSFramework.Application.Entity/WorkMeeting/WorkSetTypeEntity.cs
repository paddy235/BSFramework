using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity
{
    /// <summary>
    /// 版 本 6.1
    /// 描 述：班制类别
    /// </summary>
    [Table("wg_worksetup")]
    public class WorkSetTypeEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("id")]
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("systemname")]
        public string SystemName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("systemtype")]
        public string SystemType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("createdate")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("createuserid")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("createusername")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("deletemark")]
        public decimal DeleteMark { get; set; }
       

        #endregion


    }
}
