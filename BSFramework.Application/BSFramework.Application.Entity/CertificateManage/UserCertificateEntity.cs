using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.ComponentModel;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Application.Entity.CertificateManage
{
    /// <summary>
    /// 描 述：证件类别表
    /// </summary>
    [Table("wg_usercertificate")]
    public class UserCertificateEntity : BaseEntity
    {
        #region 实体成员 
        /// <summary>
        /// 主键id
        /// </summary>	
        [Key]
        [Column("Id")]
        public string Id { get; set; }
        /// <summary>
        /// 证件名称
        /// </summary>	
        [Column("certificatename")]
        public string CertificateName { get; set; }
        /// <summary>
        /// 初领时间
        /// </summary>	
        [Column("firsttime")]
        public DateTime firsttime { get; set; }
        /// <summary>
        ///有效期开始时间
        /// </summary>	
        [Column("effectivestarttime")]
        public DateTime effectivestarttime { get; set; }
        /// <summary>
        ///批准时间
        /// </summary>	
        [Column("approvaltime")]
        public DateTime approvaltime { get; set; }
        /// <summary>
        ///最新有效日期
        /// </summary>	
        [Column("neweffectivetime")]
        public DateTime neweffectivetime { get; set; }
        /// <summary>
        ///有效时间
        /// </summary>	
        [Column("effectivetime")]
        public DateTime effectivetime { get; set; }
        /// <summary>
        ///发证时间
        /// </summary>	
        [Column("getthetime")]
        public DateTime getthetime { get; set; }
        /// <summary>
        ///有效期结束时间
        /// </summary>	
        [Column("effectiveendtime")]
        public DateTime effectiveendtime { get; set; }
        /// <summary>
        ///复审时间
        /// </summary>	
        [Column("rechecktime")]
        public DateTime rechecktime { get; set; }
        /// <summary>
        ///证件编号
        /// </summary>	
        [Column("numbercode")]
        public string numbercode { get; set; }
        /// <summary>
        ///用户id
        /// </summary>	
        [Column("userid")]
        public string userid { get; set; }
        /// <summary>
        ///发证机关
        /// </summary>	
        [Column("issue")]
        public string issue { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>	
        [Column("createtime")]
        public DateTime createtime { get; set; }
        /// <summary>
        ///创建人
        /// </summary>	
        [Column("createuser")]
        public string createuser { get; set; }
        /// <summary>
        ///创建人id
        /// </summary>	
        [Column("createuserid")]
        public string createuserid { get; set; }
        /// <summary>
        ///修改时间
        /// </summary>	
        [Column("modifytime")]
        public DateTime modifytime { get; set; }
        /// <summary>
        ///修改人
        /// </summary>	
        [Column("modifyuser")]
        public string modifyuser { get; set; }

        /// <summary>
        ///修改人id
        /// </summary>	
        [Column("modifyuserid")]
        public string modifyuserid { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public string state { get; set; }
        [NotMapped]
        public string path { get; set; }

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
