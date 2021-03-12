using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.ComponentModel;
using System.Collections.Generic;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.CertificateManage
{
    /// <summary>
    /// 描 述：证件类别表
    /// </summary>
    [Table("wg_certificate")]
    public class CertificateEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id 
        /// </summary>	
        [Key]
        [Column("certificateid")]
        public string CertificateId { get; set; }
        /// <summary>
        /// 证件名称
        /// </summary>	

        [Column("certificatename")]
        public string CertificateName { get; set; }
        /// <summary>
        /// 证件名称
        /// </summary>	

        [Column("pcertificatename")]
        public string PCertificateName { get; set; }
        /// <summary>
        /// 证件名称
        /// </summary>	

        [Column("parentid")]
        public string ParentId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Column("sort")]
        public int Sort { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [Column("iseffective")]
        public bool IsEffective { get; set; }

        /// <summary>
        ///创建时间
        /// </summary>	
        [Column("createtime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        [Column("createuser")]
        public string CreateUser { get; set; }
        /// <summary>
        ///创建人id
        /// </summary>
        [Column("createuserid")]
        public string CreateUserId { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        [Column("modifytime")]
        public DateTime ModifyTime { get; set; }
        /// <summary>
        ///修改人
        /// </summary>
        [Column("modifyuser")]
        public string ModifyUser { get; set; }
        /// <summary>
        ///修改人id
        /// </summary>
        [Column("modifyuserid")]
        public string ModifyUserId { get; set; }



        #endregion

        #region 扩展操作

        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CertificateId = Guid.NewGuid().ToString();
            if (string.IsNullOrEmpty(this.CreateUser))
            {
                this.CreateTime = DateTime.Now;
                this.CreateUser = OperatorProvider.Provider.Current().UserName;
                this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            }

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.CertificateId = keyValue;
            if (string.IsNullOrEmpty(this.CreateUser))
            {
                this.ModifyTime = DateTime.Now;
                this.ModifyUser = OperatorProvider.Provider.Current().UserName;
                this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            }
        }
        #endregion
    }
}
