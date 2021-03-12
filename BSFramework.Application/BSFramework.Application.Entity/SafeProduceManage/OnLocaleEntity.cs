using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SafeProduceManage
{

    /// <summary>
    /// 现场终端功能踩点记录
    /// </summary>
    [Table("WG_ONLOCALE")]
    public class OnLocaleEntity : BaseEntity
    {

        /// <summary>
        /// 主键
        /// </summary>
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 责任区域
        /// </summary>
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }
        /// <summary>
        /// 责任区域
        /// </summary>
        [Column("DISTRICT")]
        public string District { get; set; }
        /// <summary>
        /// 责任区域
        /// </summary>
        [Column("DISTRICTCODE")]
        public string DistrictCode { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Column("USERID")]
        public string UserId { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        [Column("USERNAME")]
        public string UserName { get; set; }
        /// <summary>
        /// 登陆时间
        /// </summary>
        [Column("SIGNINDATE")]
        public DateTime SigninDate { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        /// 部门code
        /// </summary>
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 登陆模块
        /// </summary>
        [Column("MODULE")]
        public string Module { get; set; }

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
        /// 人脸头像
        /// </summary>
        [NotMapped]
        public string photo { get; set; }

    }
    public class OnLocaleModel
    {
        public string DeptName { get; set; }
        public string DutyType { get; set; }
        public string SigninDate { get; set; }

    }
}
