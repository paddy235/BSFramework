using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{
    [Table("Wg_SevensPictureEvaluation")]
   public class SevensPictureEvaluationEntity : BaseEntity
    {
        [Column("ID")]
        public string Id { get; set; }


        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }
        [Column("ModifyDate")]
        public DateTime ModifyDate { get; set; }
        /// <summary>
        /// 评论人
        /// </summary>
        [Column("CreateUser")]
        public string CreateUser { get; set; }
        /// <summary>
        /// 评论人的部门Id
        /// </summary>
        [Column("CreateUserDeptId")]
        public string CreateUserDeptId { get; set; }
        /// <summary>
        /// 评论人的部门名称
        /// </summary>
        [Column("CreateUserDeptName")]
        public string CreateUserDeptName { get; set; }
        /// <summary>
        /// 评论人的名称
        /// </summary>
        [Column("CreateUserName")]
        public string CreateUserName { get; set; }
        [Column("ModifyUserId")]
        public string ModifyUserId { get; set; }
        [Column("ModifyUserName")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        [Column("Point")]
        public double Point { get; set; }

        /// <summary>
        /// 评论内容
        /// </summary>
        [Column("Content")]
        public string Content { get; set; }

        /// <summary>
        /// 评论的班组
        /// </summary>
        [Column("EvaluationDept")]
        public string EvaluationDept { get; set; }
        /// <summary>
        /// 所评论的数据的id
        /// </summary>
        [Column("EvaluateDataId")]
        public string EvaluateDataId { get; set; }

        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK1")]
        public string BK1 { get; set; }

        /// <summary>
        /// 备用字段
        /// </summary>
        [Column("BK2")]
        public string BK2 { get; set; }

    }
}
