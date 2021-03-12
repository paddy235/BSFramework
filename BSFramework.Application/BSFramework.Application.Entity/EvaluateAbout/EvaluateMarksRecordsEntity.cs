using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EvaluateAbout
{
    /// <summary>
    /// 考评加减分记录表
    /// </summary>
    [Table("WG_EVALUATEMARKSRECORDS")]
    public class EvaluateMarksRecordsEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 考评项的Id
        /// </summary>
        [Column("EVALUATEITEMID")]
        public string EvaluateItemId { get; set; }
        /// <summary>
        /// 加/减分的值
        /// </summary>
        [Column("SCORE")]
        public decimal Score { get; set; }
        /// <summary>
        /// 加/减分的原因
        /// </summary>
        [Column("CAUSE")]
        public string Cause { get; set; }
        /// <summary>
        /// 考评人ID
        /// </summary>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 考评人名称
        /// </summary>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 考评人所在的部门的Id
        /// </summary>
        [Column("CREATEUSERDEPTID")]
        public string CreateUserDeptId { get; set; }
        /// <summary>
        /// 考评人所在部门的名称
        /// </summary>
        [Column("CREATEUSERDEPTNAME")]
        public string CreateUserDeptName { get; set; }
        /// <summary>
        /// 考评时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        [Column("MODIFYUSER")]
        public string ModifyUser { get; set; }

        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }

        [Column("MODIFYUSERDEPTID")]
        public string ModifyUserDeptId { get; set; }
        [Column("MODIFYUSERDEPTNAME")]
        public string ModifyUserDeptName { get; set; }
        /// <summary>
        /// 是否是厂级（公司级）数据 1-是 0-不是
        /// </summary>
        [Column("ISORG")]
        public int IsOrg { get; set; }
        public override void Create()
        {
            Id = Guid.NewGuid().ToString();
            CreateDate = DateTime.Now;
            var user = OperatorProvider.Provider.Current();
            CreateUserId = user.UserId;
            CreateUserName = user.UserName;
            CreateUserDeptId = user.DeptId;
            CreateUserDeptName = user.DeptName;
            ModifyDate = DateTime.Now;
        }
        public override void Modify(string keyValue)
        {
            Id = keyValue;
            var user = OperatorProvider.Provider.Current();
            ModifyDate = DateTime.Now;
            ModifyUser = user.UserName;
            ModifyUserId = user.UserId;
            ModifyUserDeptId = user.DeptId;
            ModifyUserDeptName = user.DeptName;
        }
    }

    public class EvaluateRecord
    {
        public decimal Score { get; set; }
        public string Cause { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal? Weight { get; set; }
        public string GroupName { get; set; }
    }
}
