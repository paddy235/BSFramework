using BSFramework.Application.Code;
using BSFramework.Application.Entity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
namespace System
{
    /// <summary>
    /// 考评修正记录表
    /// </summary>
    [Table("wg_evaluaterevise")]
    public class EvaluateReviseEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("Id")]
        [Description("主键")]
        public string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CreateDate")]
        [Description("创建时间")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("CreateUser")]
        [Description("创建人")]
        public string CreateUser { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        [Column("CreateUserId")]
        [Description("创建人Id")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 考评要素ID
        /// </summary>
        [Column("CategoryId")]
        [Description("考评要素ID")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 考评要素
        /// </summary>
        [Column("Category")]
        [Description("考评要素")]
        public string Category { get; set; }

        /// <summary>
        /// 标准分
        /// </summary>
        [Column("StandardScore")]
        [Description("标准分")]
        public decimal StandardScore { get; set; }

        /// <summary>
        /// 部门ID
        /// </summary>
        [Column("DepartmentId")]
        [Description("部门ID")]
        public string DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [Column("DepartmentName")]
        [Description("部门名称")]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 班组Id
        /// </summary>
        [Column("GroupId")]
        [Description("班组Id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 班组名称
        /// </summary>
        [Column("GroupName")]
        [Description("班组名称")]
        public string GroupName { get; set; }

        /// <summary>
        /// 部门打分
        /// </summary>
        [Column("DeptScore")]
        [Description("部门打分")]
        public decimal DeptScore { get; set; }

        /// <summary>
        /// 部门打分原因
        /// </summary>
        [Column("DeptCause")]
        [Description("部门打分原因")]
        public string DeptCause { get; set; }

        /// <summary>
        /// 部门考评人Id
        /// </summary>
        [Column("DeptEvaluateUserId")]
        [Description("部门考评人Id")]
        public string DeptEvaluateUserId { get; set; }

        /// <summary>
        /// 部门考评人名称
        /// </summary>
        [Column("DeptEvaluteUser")]
        [Description("部门考评人名称")]
        public string DeptEvaluteUser { get; set; }

        /// <summary>
        /// 考评部门修正后的分数
        /// </summary>
        [Column("ReviseScore")]
        [Description("考评部门修正后的分数")]
        public decimal ReviseScore { get; set; }

        /// <summary>
        /// 修正原因
        /// </summary>
        [Column("ReviseCause")]
        [Description("修正原因")]
        public string ReviseCause { get; set; }

        /// <summary>
        /// 修正人Id
        /// </summary>
        [Column("ReviseUserId")]
        [Description("修正人Id")]
        public string ReviseUserId { get; set; }

        /// <summary>
        /// 修正人名称
        /// </summary>
        [Column("ReviseUser")]
        [Description("修正人名称")]
        public string ReviseUser { get; set; }

        /// <summary>
        /// 考评内容Id
        /// </summary>
        [Column("EvaluteContentId")]
        [Description("考评内容Id")]
        public string EvaluteContentId { get; set; }

        /// <summary>
        /// 考评内容
        /// </summary>
        [Column("EvaluteContent")]
        [Description("考评内容")]
        public string EvaluteContent { get; set; }

        /// <summary>
        /// 是否是删除数据,1是 0不是
        /// </summary>
        [Column("IsDeleteType")]
        [Description("是否是删除数据,1是 0不是")]
        public int IsDeleteType { get; set; }

        /// <summary>
        /// 考评Id
        /// </summary>
        [Column("EvaluateId")]
        [Description("考评Id")]
        public string EvaluateId { get; set; }

        /// <summary>
        /// 考评名称
        /// </summary>
        [Column("EvaluateSeason")]
        [Description("考评名称")]
        public string EvaluateSeason { get; set; }


        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUser = OperatorProvider.Provider.Current().UserName;
        }
    }
}
