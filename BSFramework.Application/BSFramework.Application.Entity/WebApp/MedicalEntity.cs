using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WebApp
{
    [Table("wg_usermedical")]
    public class MedicalEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Key]
        [Column("medicalId")]
        public string MedicalId { get; set; }
        /// <summary>
        /// 体检类型名称
        /// </summary>	
        [Column("medicaltype")]
        public string MedicalType { get; set; }
        /// <summary>
        ///体检时间
        /// </summary>	
        [Column("medicaltime")]
        public DateTime MedicalTime { get; set; }
        /// <summary>
        ///体检类型id
        /// </summary>	
        [Column("medicaltypeId")]
        public string MedicalTypeId { get; set; }
        /// <summary>
        ///机构
        /// </summary>	
        [Column("organization")]
        public string Organization { get; set; }
        /// <summary>
        ///体检结果
        /// </summary>	
        [Column("healthresult")]
        public string HealthResult { get; set; }
        /// <summary>
        ///体检结果类型id
        /// </summary>	
        [Column("healthresultId")]
        public string HealthResultId { get; set; }
        ///// <summary>
        /////用户id
        ///// </summary>	
        //[Column("userid")]
        //public string userid { get; set; }
        /// <summary>
        ///结果备注
        /// </summary>	
        [Column("resultdetail")]
        public string ResultDetail { get; set; }
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
        /// <summary>
        ///备注
        /// </summary>	
        [Column("remark")]
        public string remark { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
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

