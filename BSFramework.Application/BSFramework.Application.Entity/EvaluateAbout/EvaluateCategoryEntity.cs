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
    /// √Ë  ˆ£∫∞‡«∞∞‡∫Ûª·
    /// </summary>
    [Table("wg_evaluatecategory")]
    public class EvaluateCategoryEntity : BaseEntity
    {
        public string CategoryId { get; set; }
        public string Category { get; set; }
        public DateTime CreateTime { get; set; }
        public string ParentCategoryId { get; set; }
        public int SortCode { get; set; }

    }
}