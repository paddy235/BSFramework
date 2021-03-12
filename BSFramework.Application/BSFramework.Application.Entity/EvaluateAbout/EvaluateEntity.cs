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
    [Table("wg_evaluate")]
    public class EvaluateEntity : BaseEntity
    {
        public string EvaluateId { get; set; }
        public string EvaluateSeason { get; set; }
        public string EvaluateStatus { get; set; }
        public DateTime? PublishDate { get; set; }
        public DateTime CreateTime { get; set; }
        public string EvaluateUserId { get; set; }
        public string EvaluateUser { get; set; }
        public DateTime? LimitTime { get; set; }
        public string EvaluateCycle { get; set; }
        public bool IsEvaluated { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsPublished { get; set; }
        /// <summary>
        /// 考评范围，单位ID逗号隔开
        /// </summary>
        public string DeptScope { get; set; }
        /// <summary>
        /// 考评范围的单位名称,逗号隔开
        /// </summary>
        public string DeptScopeName { get; set; }

        [NotMapped]
        public List<EvaluateGroupEntity> Groups { get; set; }
        [NotMapped]
        public string CanScore { get; set; }
        [NotMapped]
        public string CanEdit { get; set; }
        [NotMapped]
        public string CanCalc { get; set; }
        [NotMapped]
        public string CanDel { get; set; }
        /// <summary>
        /// 是否能发布
        /// </summary>
        [NotMapped]
        public string CanPublish { get; set; }
    }

    public class EvaluateCalcEntity
    {
        public string Season { get; set; }
        public string SeasonId { get; set; }
        public int Seq { get; set; }
        public IList<EvaluateItemCalcEntity> Data { get; set; }
    }

    public class EvaluateItemCalcEntity
    {
        public string Category { get; set; }
        public decimal ActualScore { get; set; }
        public decimal Score { get; set; }
        public decimal Pct { get; set; }
    }

    public class EvaluateScoreItemEntity
    {
        public IList<FileInfoEntity> files { get; set; }
        public string Category { get; set; }
        public string CategoryItemId { get; set; }
        public string Reason { get; set; }
        public string ItemContent { get; set; }
        public decimal ActualScore { get; set; }
        public decimal Score { get; set; }
        public decimal? WeightScore { get; set; }
        public string CategoryId { get; set; }
    }

    public class EvaluateScoreDetail
    {
        /// <summary>
        /// 分数类型 加分、扣分
        /// </summary>
        public string ScoreType { get; set; }
        /// <summary>
        /// 加/扣分 的分数
        /// </summary>
        public decimal MarksScore { get; set; }

        /// <summary>
        /// 标准分
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        ///实际得分
        /// </summary>
        public decimal ActualScore { get; set; }
        /// <summary>
        /// 权重分
        /// </summary>
        public decimal? WeightScore { get; set; }
        /// <summary>
        /// 加减分原因
        /// </summary>
        public EvaluateMarksRecordsEntity Records { get; set; }
        /// <summary>
        /// 考评标准
        /// </summary>
        public string ItemStandard { get; set; }
        public string Category { get; set; }
    }

}