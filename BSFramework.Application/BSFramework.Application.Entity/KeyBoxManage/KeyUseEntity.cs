using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.KeyboxManage
{
    /// <summary>
    /// 钥匙使用
    /// </summary>
    [Table("WG_KEYBOX")]
    public class KeyUseEntity : BaseEntity
    {  /// <summary>
       ///主键id
       /// </summary>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 钥匙id
        /// </summary>		
        [Column("KEYID")]
        public string KeyId { get; set; }
        /// <summary>
        /// 钥匙code
        /// </summary>		
        [Column("KEYCODE")]
        public string KeyCode { get; set; }
        /// <summary>
        /// 钥匙地点
        /// </summary>		
        [Column("KEYPLACE")]
        public string KeyPlace { get; set; }
        /// <summary>
        /// 专业类别 
        /// </summary>		
        [Column("CATEGORY")]
        public string Category { get; set; }
        /// <summary>
        /// 专业类别
        /// </summary>		
        [Column("CATEGORYID")]
        public string CategoryId { get; set; }
        /// <summary>
        /// 理由
        /// </summary>		
        [Column("REASON")]
        public string Reason { get; set; }
        
        /// <summary>
        /// 借出人
        /// </summary>		
        [Column("LOANUSER")]
        public string LoanUser { get; set; }
        /// <summary>
        /// 借出id
        /// </summary>		
        [Column("LOANUSERID")]
        public string LoanUserId { get; set; }
        /// <summary>
        /// 借出time
        /// </summary>		
        [Column("LOANDATE")]
        public DateTime? LoanDate { get; set; }
        /// <summary>
        /// 借用
        /// </summary>		
        [Column("BORROWUSER")]
        public string BorrowUser { get; set; }
        /// <summary>
        /// 借用id
        /// </summary>		
        [Column("BORROWUSERID")]
        public string BorrowUserId { get; set; }
        /// <summary>
        /// 归还办理人
        /// </summary>		
        [Column("OPERATEUSER")]
        public string OperateUser { get; set; }
        /// <summary>
        /// 归还办理人
        /// </summary>		
        [Column("OPERATEUSERID")]
        public string OperateUserId { get; set; }
        /// <summary>
        /// 归还time
        /// </summary>		
        [Column("SENDBACKDATE")]
        public DateTime? SendBackDate { get; set; }
        /// <summary>
        /// 所属班组id
        /// </summary>		
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 所属班组code
        /// </summary>		
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 所属班组
        /// </summary>		
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
    }
}
