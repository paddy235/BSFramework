using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.TaskManage
{
    /// <summary>
    /// 未签到记录
    /// </summary>
    public class UnSigninEntity
    {
        [Column("UNSIGNINID")]
        public string UnSigninId { get; set; }
        [Column("UNSIGNINDATE")]
        public DateTime UnSigninDate { get; set; }
        [Column("USERID")]
        public string UserId { get; set; }
        [Column("USERNAME")]
        public string UserName { get; set; }
        [Column("DUTYDEPARTMENTID")]
        public string DutyDepartmentId { get; set; }
        [Column("DUTYDEPARTMENTNAME")]
        public string DutyDepartmentName { get; set; }
        [Column("CATEGORYID")]
        public string CategoryId { get; set; }
        [Column("CATEGORYNAME")]
        public string CategoryName { get; set; }
        [Column("DISTRICTID")]
        public string DistrictId { get; set; }
        [Column("DISTRICTNAME")]
        public string DistrictName { get; set; }
    }
}
