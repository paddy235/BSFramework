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
    ///班组创新管理
    /// </summary>
    [Table("wg_workinnovation")]
    public class WorkInnovationEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("innovationid")]
        public String innovationid { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Column("deptid")]
        public String deptid { get; set; }
        /// <summary>
        /// 部门name
        /// </summary>
        [Column("deptname")]
        public String deptname { get; set; }
        /// <summary>
        /// 提案名称
        /// </summary>
        [Column("name")]
        public String name { get; set; }
        /// <summary>
        /// 现在
        /// </summary>
        [Column("statusquo")]
        public String statusquo { get; set; }
        /// <summary>
        /// 提议
        /// </summary>
        [Column("proposed")]
        public String proposed { get; set; }
        /// <summary>
        /// 提议时间
        /// </summary>
        [Column("reporttime")]
        public DateTime reporttime { get; set; }
        /// <summary>
        /// 提议人
        /// </summary>
        [Column("reportuser")]
        public String reportuser { get; set; }
        /// <summary>
        /// 提议
        /// </summary>
        [Column("reportuserid")]
        public String reportuserid { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        [Column("aduitstate")]
        public String aduitstate { get; set; }
        /// <summary>
        /// 审核结论
        /// </summary>
        [Column("aduitresult")]
        public String aduitresult { get; set; }
        /// <summary>
        /// 回退数量
        /// </summary>
        [Column("returnnum")]
        public int  returnnum { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> statusquoPhoto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> proposedFile { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> proposedPhoto { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public List<WorkInnovationAuditEntity> audit { get; set; }
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
      
        }
        /// <summary>
        /// 清理文件
        /// </summary>
        public void clearFile() {
            this.proposedFile = null;
            this.proposedPhoto = null;
            this.statusquoPhoto = null;
            this.audit = null;
        }
        #endregion
    }
}
