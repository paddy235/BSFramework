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
    /// �� ��������
    /// </summary>
    [Table("wg_evaluategroup")]
    public class EvaluateGroupEntity : BaseEntity
    {
        public string EvaluateGroupId { get; set; }
        public string EvaluateId { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public bool IsSubmitted { get; set; }
        /// <summary>
        /// ���ε÷�
        /// </summary>
        [NotMapped]
        public decimal? ActualScore { get; set; }
        public DateTime CreateTime { get; set; }
        [NotMapped]
        public List<EvaluateItemEntity> Items { get; set; }
        [NotMapped]
        public decimal? Pct { get; set; }
        [NotMapped]
        //�ƺ�
        public string TitleName { get; set; }
    }
    public class Group 
    {
        public string GroupId { get; set; }
        public string GroupName { get; set; }
        public decimal Score { get; set; } //��׼��
        public decimal Score1 { get; set; }//���ε÷�
        public decimal Score2 { get; set; }//�ϴε÷�
        public string Percent { get; set; }
        public string Percent1 { get; set; }
        public int Index { get; set; }
        public string DeptId { get; set; }
        /// <summary>
        /// ����ʲô�ֶε�����������  HM
        /// </summary>
        public string  GroupBy { get; set; }
        /// <summary>
        /// �ƺ�
        /// </summary>
        public string TitleName { get; set; }
        public string TitleId { get; set; }

        public string Tid { get; set; }
    }

    public class GroupIndex
    {
        public string Category { get; set; }
        public string GroupName { get; set; }
        public decimal Score { get; set; } //��׼��
        public decimal Score1 { get; set; }//���ε÷�
        public decimal Score2 { get; set; }//�ϴε÷�
        public string Percent { get; set; }
        public string Percent1 { get; set; }
        public int Index { get; set; }
        public string DeptId { get; set; }
    }
    public class DeductInfo 
    {
        public string Category { get; set; }
        public string ItemContent { get; set; }
        public string GroupNames { get; set; }
        public decimal Times { get; set; }
        public double Percent { get; set; }

        public string CreateTime { get; set; }
    }
}