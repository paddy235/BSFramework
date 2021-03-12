using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SafeProduceManage
{
    /// <summary>
    /// 安全文明生成检查
    /// </summary>
    [Table("WG_SAFEPRODUCE")]
    public class SafeProduceEntity : BaseEntity
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 登记日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 登记用户主键
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 登记用户
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
        /// 办理日期
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 办理用户主键
        /// </summary>		
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 办理用户
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
        /// 责任班组id
        /// </summary>		
        [Column("DUTYDEPTID")]
        public string DutyDeptId { get; set; }
        /// <summary>
        /// 责任班组code
        /// </summary>		
        [Column("DUTYDEPTCODE")]
        public string DutyDeptCode { get; set; }
        /// <summary>
        /// 责任班组
        /// </summary>		
        [Column("DUTYDEPTNAME")]
        public string DutyDeptName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>		
        [Column("DESCRIBE")]
        public string Describe { get; set; }
        /// <summary>
        /// 措施
        /// </summary>		
        [Column("MEASURE")]
        public string Measure { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>		
        [Column("STATE")]
        public string State { get; set; }
        /// <summary>
        /// 处理情况
        /// </summary>		
        [Column("SITUATION")]
        public string Situation { get; set; }

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

    }
}
