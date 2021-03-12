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
    /// 合理化建议
    /// </summary>
    [Table("wg_advice")]
    public class AdviceEntity : BaseEntity
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Column("adviceid")]
        public String adviceid { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Column("deptname")]
        public String deptname { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Column("deptid")]
        public String deptid { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Column("username")]
        public String username { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Column("userid")]
        public String userid { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [Column("title")]
        public String title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        [Column("content")]
        public String content { get; set; }
        /// <summary>
        /// 填报时间
        /// </summary>
        [Column("reporttime")]
        public DateTime reporttime { get; set; }
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
        /// 类别
        /// </summary>
        [Column("advicetype")]
        public string advicetype { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [NotMapped]
        public string tousername { get; set; }
        /// <summary>
        /// 类别
        /// </summary>
        [NotMapped]
        public string touserid { get; set; }
        /// <summary>
        /// 审核数据
        /// </summary>
        [NotMapped]
        public List<AdviceAuditEntity> audit
        {
            get; set;

        }
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
