using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SetManage
{
    /// <summary>
    /// 描 述：危险因素
    /// </summary>
    [Table("WG_RISKFACTORSET")]
    public class RiskFactorSetEntity : BaseEntity
    {

        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        [Column("CREATEDEPTID")]
        public string CreateDeptId { get; set; }
        [Column("CREATEDEPTCODE")]
        public string CreateDeptCode { get; set; }
        [Column("CREATEDEPTNAME")]
        public string CreateDeptName { get; set; }
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Column("DEPTNAME")]

        public string DeptName { get; set; }
        /// <summary>
        /// 危险因素
        /// </summary>
        [Column("CONTENT")]
        [Display(Name = "危险因素"),
            Required(ErrorMessage = "请输入危险因素"),
            MinLength(1),
            MaxLength(500)]
        public string Content { get; set; }

        #endregion

        /// <summary>
        /// 危险因素下的防范措施列表
        /// </summary>
        [NotMapped]
        [Display(Name = "防范措施")]
        public List<MeasureSetEntity> measures { get; set; }
        /// <summary>
        /// 需要删除的防范措施ID
        /// </summary>
        [NotMapped]
        public List<string> measureids { get; set; }

        public override void Create()
        {
            this.ID = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.CreateDeptId = OperatorProvider.Provider.Current().DeptId;
            this.CreateDeptCode = OperatorProvider.Provider.Current().DeptCode;
            this.CreateDeptName = OperatorProvider.Provider.Current().DeptName;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId; ;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }

        public override void Modify(string keyValue)
        {
            this.ID = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
    }
}
