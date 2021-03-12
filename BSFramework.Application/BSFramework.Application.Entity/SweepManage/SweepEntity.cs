using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SweepManage
{
    /// <summary>
    /// 保洁管理
    /// </summary>
    [Table("WG_SWEEP")]
    public class SweepEntity : BaseEntity
    {

        /// <summary>
        ///主键id
        /// </summary>
        [Column("ID")]
        public string ID { get; set; }
        /// <summary>
        /// 保洁日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 保洁责任人
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 保洁责任人
        /// </summary>		
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 责任人类型
        /// </summary>		
        [Column("DUTYTYPE")]
        public string DutyType { get; set; }
        /// <summary>
        /// 责任人类型id
        /// </summary>		
        [Column("DUTYTYPEID")]
        public string DutyTypeId { get; set; }

        /// <summary>
        /// 管理责任人
        /// </summary>		
        [Column("DUTYUSER")]
        public string DutyUser { get; set; }
        /// <summary>
        /// 管理责任人id
        /// </summary>		
        [Column("DUTYUSERID")]
        public string DutyUserId { get; set; }
        /// <summary>
        /// 检测日期
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 质量检测人
        /// </summary>		
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 质量检测人
        /// </summary>		
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 质量检测班组ID
        /// </summary>		
        [Column("QUALITYDEPTID")]
        public string QualityDeptId { get; set; }
        /// <summary>
        /// 质量检测班组ID
        /// </summary>		
        [Column("QUALITYDEPTCODE")]
        public string QualityDeptCode { get; set; }
        /// <summary>
        /// 质量检测班组ID
        /// </summary>		
        [Column("QUALITYDEPTNAME")]
        public string QualityDeptName { get; set; }
        /// <summary>
        /// 保洁责任人所属班组id
        /// </summary>		
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 保洁责任人所属班组code
        /// </summary>		
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 保洁责任人所属班组
        /// </summary>		
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        /// 区域
        /// </summary>	
        [Column("DISTRICT")]
        public string District { get; set; }
        /// <summary>
        /// 区域
        /// </summary>	
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }

        /// <summary>
        /// 区域
        /// </summary>	
        [Column("DISTRICTCODE")]
        public string DistrictCode { get; set; }
        /// <summary>
        /// 处理情况
        /// </summary>	
        [Column("SITUATION")]
        public string Situation { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>	
        [Column("STATE")]
        public string State { get; set; }

        /// <summary>
        /// 保洁项
        /// </summary>	
      [NotMapped]
        public List<SweepItemEntity> Items { get; set; }
    }
}
