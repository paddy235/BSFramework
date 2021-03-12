using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.CarcOrCardManage
{
    /// <summary>
    /// 描 述：Carc 手袋卡 表
    /// </summary>
    [Table("WG_CCARD")]
    public class CCardEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id 
        /// </summary>	
        [Key]
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>	

        [Column("WORKNAME")]
        public string WorkName { get; set; }
        /// <summary>
        /// 任务区域
        /// </summary>	

        [Column("WORKAREA")]
        public string WorkArea { get; set; }
        ///主要操作步骤
        /// </summary>
        [Column("MAINOPERATION")]
        public string MainOperation { get; set; }


        /// <summary>
        ///组织id
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        ///组织code
        /// </summary>
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        ///组织名称
        /// </summary>
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        ///岗位
        /// </summary>
        [Column("DUTYID")]
        public string DutyId { get; set; }
        /// <summary>
        ///岗位
        /// </summary>
        [Column("DUTYNAME")]
        public string DutyName { get; set; }
        /// <summary>
        ///创建人时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        ///创建人id
        /// </summary>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        ///修改人时间
        /// </summary>
        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }
        /// <summary>
        ///修改人id
        /// </summary>
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        ///修改人
        /// </summary>
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 风险标识
        /// </summary>
        [NotMapped]
        public IList<CDangerousEntity> CDangerousList { get; set; }

        #endregion

        #region 扩展操作

        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
          
        }
        #endregion
    }
}
