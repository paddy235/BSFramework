using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.PublicInfoManage;


namespace BSFramework.Application.Entity.InnovationManage
{
    /// <summary>
    /// 班组创新审核
    /// </summary>
    [Table("wg_workinnovationaudit")]
   public class WorkInnovationAuditEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("auditid")]
        public string auditid { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        [Column("userid")]
        public string userid { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [Column("username")]
        public string username { get; set; }
        /// <summary>
        /// 提交状态
        /// </summary>
        [Column("state")]
        public string state { get; set; }
        /// <summary>
        /// 提交时间
        /// </summary>
        [Column("submintdate")]
        public DateTime? submintdate { get; set; }
        /// <summary>
        /// 审核意见
        /// </summary>
        [Column("opinion")]
        public string opinion { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        [Column("sort")]
        public int sort { get; set; }

        /// <summary>
        /// 关联
        /// </summary>
        [Column("innovationid")]
        public String innovationid{ get; set; }

        /// <summary>
        /// 是否特殊审核人
        /// </summary>
        [Column("isspecial")]
        public bool isspecial { get; set; }
        
    }
}
