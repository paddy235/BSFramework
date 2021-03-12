﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    /// <summary>
    /// 活动人员
    /// </summary>
    [Table("wg_activityrecord")]
    public class ActivityRecordEntity : BaseEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityPersonId")]
        public string ActivityRecordId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("ActivityId")]
        public string ActivityId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Content")]
        public string Content { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("Judge")]
        public string Judge { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ActivityEntity Activity { get; set; }
    }
}
