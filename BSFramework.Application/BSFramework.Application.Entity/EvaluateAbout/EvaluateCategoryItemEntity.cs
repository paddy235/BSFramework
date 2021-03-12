using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Entity.EvaluateAbout
{
    /// <summary>
    /// 描 述：班前班后会
    /// </summary>
    [Table("wg_evaluatecategoryitem")]
    public class EvaluateCategoryItemEntity : BaseEntity
    {
        public string ItemId { get; set; }
        public string CategoryId { get; set; }
        /// <summary>
        /// 考评内容
        /// </summary>
        public string ItemContent { get; set; }
        /// <summary>
        /// 考评标准
        /// </summary>
        public string ItemStandard { get; set; }
        public int Score { get; set; }
        public string EvaluateDept { get; set; }

        //public string EvaluateDeptId { get; set; }
        public string UseDept { get; set; }
        public string UseDeptId { get; set; }
        public string CreateUserId { get; set; }
        public DateTime CreateTime { get; set; }
        [NotMapped]
        public EvaluateCategoryEntity Category { get; set; }
    }
}