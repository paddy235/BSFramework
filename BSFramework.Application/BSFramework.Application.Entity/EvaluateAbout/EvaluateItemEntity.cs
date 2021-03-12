using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;
using BSFramework.Application.Entity.EvaluateAbout;

namespace BSFramework.Entity.EvaluateAbout
{
    /// <summary>
    /// 描 述：考评
    /// </summary>
    [Table("wg_evaluateitem")]
    public class EvaluateItemEntity : BaseEntity
    {
        public string EvaluateItemId { get; set; }
        public string EvaluateGroupId { get; set; }
        public string EvaluateContentId { get; set; }
        public string EvaluateContent { get; set; }
        public decimal Score { get; set; }
        public decimal ActualScore { get; set; }
        public string EvaluateDept { get; set; }
        public string EvaluatePerson { get; set; }
        public DateTime? EvaluateTime { get; set; }
        public DateTime CreateTime { get; set; }
        public string Reason { get; set; }
        [NotMapped]
        public decimal? Pct { get; set; }
        /// <summary>
        /// 分类ID，非数据库字段
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// 计算权重后的分数
        /// </summary>
        public decimal? WeightScore { get; set; }
        public string WeightId { get; set; }
        [NotMapped]
        public List<EvaluateMarksRecordsEntity> MarksRecord { get; set; }
    }
}