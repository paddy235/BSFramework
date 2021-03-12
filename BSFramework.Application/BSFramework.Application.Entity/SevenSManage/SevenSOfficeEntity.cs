using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{

    /// <summary>
    /// 精益管理提交审核表
    /// </summary>
    [Table("wg_sevensoffice")]
    public class SevenSOfficeEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("id")]
        public string id { get; set; }
        /// <summary>
        /// 提案名称
        /// </summary>
        [Column("name")]
        public string name { get; set; }
        /// <summary>
        /// 现状描述
        /// </summary>
        [Column("statusquo")]
        public string statusquo { get; set; }
        /// <summary>
        /// 提议描述
        /// </summary>
        [Column("proposed")]
        public string proposed { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Column("aduitstate")]
        public string aduitstate { get; set; }
        /// <summary>
        /// 提案状态
        /// </summary>
        [Column("aduitresult")]
        public string aduitresult { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createdate")]
        public DateTime? createdate { get; set; }
        /// <summary>
        /// 创建人员id
        /// </summary>
        [Column("createuserid")]
        public string createuserid { get; set; }
        /// <summary>
        /// 创建人员名称
        /// </summary>
        [Column("createusername")]
        public string createusername { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Column("deptname")]
        public string deptname { get; set; }
        /// <summary>
        /// 上级部门名称
        /// </summary>
        [Column("parentname")]
        public string parentname { get; set; }
        /// <summary>
        /// 部门id 
        /// </summary>
        [Column("deptid")]
        public string deptid { get; set; }
        /// <summary>
        /// 上级部门id
        /// </summary>
        [Column("parentid")]
        public string parentid { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        [Column("modifydate")]
        public DateTime? modifydate { get; set; }

        /// <summary>
        /// 审核数据
        /// </summary>
        [NotMapped]
        public List<SevenSOfficeAuditEntity> audit
        {
            get;set;

        }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> proposedFiles
        {
            get; set;
        }

        /// <summary>
        /// 审核数据
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> statusquoFiles
        {
            get; set;
        }

    }
}
