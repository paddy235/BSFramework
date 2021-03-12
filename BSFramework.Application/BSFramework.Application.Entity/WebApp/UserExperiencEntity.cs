using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.WebApp
{
    [Table("wg_userexperienc")]
    public class UserExperiencEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Key]
        [Column("experiencid")]
        public string ExperiencId { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>	
        [Column("starttime")]
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>	
        [Column("endtime")]
        public string EndTime { get; set; }
        /// <summary>
        /// 是否至今
        /// </summary>	
        [Column("isend")]
        public bool Isend { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>	
        [Column("department")]
        public string Department { get; set; }
        /// <summary>
        /// 公司
        /// </summary>	
        [Column("commpany")]
        public string Commpany { get; set; }
        /// <summary>
        /// 班组
        /// </summary>	
        [Column("team")]
        public string Team { get; set; }
        /// <summary>
        /// 职务
        /// </summary>	
        [Column("jobs")]
        public string Jobs { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>	
        [Column("position")]
        public string Position { get; set; }
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
        ///能否修改
        /// </summary>	
        [Column("isupdate")]
        public bool isupdate { get; set; }

        #endregion
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.ExperiencId))
            {
                this.ExperiencId = Guid.NewGuid().ToString();
            }
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

