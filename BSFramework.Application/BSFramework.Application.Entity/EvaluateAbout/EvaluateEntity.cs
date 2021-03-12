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
    /// �� ��������
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
        /// ������Χ����λID���Ÿ���
        /// </summary>
        public string DeptScope { get; set; }
        /// <summary>
        /// ������Χ�ĵ�λ����,���Ÿ���
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
        /// �Ƿ��ܷ���
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
        /// �������� �ӷ֡��۷�
        /// </summary>
        public string ScoreType { get; set; }
        /// <summary>
        /// ��/�۷� �ķ���
        /// </summary>
        public decimal MarksScore { get; set; }

        /// <summary>
        /// ��׼��
        /// </summary>
        public decimal Score { get; set; }
        /// <summary>
        ///ʵ�ʵ÷�
        /// </summary>
        public decimal ActualScore { get; set; }
        /// <summary>
        /// Ȩ�ط�
        /// </summary>
        public decimal? WeightScore { get; set; }
        /// <summary>
        /// �Ӽ���ԭ��
        /// </summary>
        public EvaluateMarksRecordsEntity Records { get; set; }
        /// <summary>
        /// ������׼
        /// </summary>
        public string ItemStandard { get; set; }
        public string Category { get; set; }
    }

}