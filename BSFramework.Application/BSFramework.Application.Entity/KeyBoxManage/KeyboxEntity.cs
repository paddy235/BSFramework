using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.KeyboxManage
{
    /// <summary>
    /// 钥匙管理
    /// </summary>
    [Table("WG_KEYBOX")]
    public class KeyBoxEntity : BaseEntity
    {
        /// <summary>
        ///主键id
        /// </summary>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>		
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>		
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>		
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }

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
        /// 借出状态 
        /// </summary>		
        [Column("STATE")]
        public bool State { get; set; }

        /// <summary>
        /// 序号 
        /// </summary>		
        [Column("SORT")]
        public string Sort { get; set; }
    }
}
