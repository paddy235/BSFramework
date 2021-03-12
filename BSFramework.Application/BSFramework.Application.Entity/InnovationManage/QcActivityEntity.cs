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
    /// qc活动
    /// </summary>
    [Table("wg_qcactivity")]
    public class QcActivityEntity : BaseEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column("qcid")]
        public String qcid { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        [Column("deptid")]
        public String deptid { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        [Column("deptname")]
        public String deptname { get; set; }
        /// <summary>
        /// 小组名称
        /// </summary>
        [Column("groupname")]
        public String groupname { get; set; }
        /// <summary>
        /// 注册时间
        /// </summary>
        [Column("registrationtime")]
        public DateTime registrationtime { get; set; }
        /// <summary>
        /// 成立时间
        /// </summary>
        [Column("setuptime")]
        public DateTime setuptime { get; set; }
        /// <summary>
        /// 小组code
        /// </summary>
        [Column("groupcode")]
        public String groupcode { get; set; }
        /// <summary>
        /// 小组类型
        /// </summary>
        [Column("grouptype")]
        public String grouptype { get; set; }
        /// <summary>
        /// 课题名称
        /// </summary>
        [Column("subjectname")]
        public String subjectname { get; set; }
        /// <summary>
        /// 课题code
        /// </summary>
        [Column("subjectcode")]
        public String subjectcode { get; set; }
        /// <summary>
        /// 课题时间
        /// </summary>
        [Column("subjecttime")]
        public DateTime subjecttime { get; set; }
        /// <summary>
        /// 组长
        /// </summary>
        [Column("groupbossid")]
        public String groupbossid { get; set; }
        /// <summary>
        /// 组长
        /// </summary>
        [Column("groupboss")]
        public String groupboss { get; set; }
        /// <summary>
        /// 组员
        /// </summary>
        [Column("groupperson")]
        public String groupperson { get; set; }
        /// <summary>
        /// 组员
        /// </summary>
        [Column("grouppersonid")]
        public String grouppersonid { get; set; }
        /// <summary>
        /// 课题状态
        /// </summary>
        [Column("subjectstate")]
        public String subjectstate { get; set; }

        /// <summary>
        /// 工作的人员班组
        /// </summary>
        [Column("workdeptid")]
        public String workdeptid { get; set; }
        ///// <summary>
        ///// 审核数据
        ///// </summary>
        //[NotMapped]
        //public List<QcAuditEntity> audit
        //{
        //    get; set;

        //}
        /// <summary>
        /// 文件
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Files
        {
            get; set;
        }

        /// <summary>
        /// 图片
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Photos
        {
            get; set;
        }
    }
}
