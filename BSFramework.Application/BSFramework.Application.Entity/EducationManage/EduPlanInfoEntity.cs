using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BSFramework.Application.Entity.EducationManage
{
    [Table("wg_eduplaninfo")]
    public class EduPlanInfoEntity
    {
        [Column("ID")]
        public String ID { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateUserId")]
        public String CreateUserId { get; set; }

        [Column("createDeptid")]
        public String createDeptid { get; set; }

        [Column("createDeptName")]
        public String createDeptName { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>		
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>		
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>		
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }


        [Column("MODIFYDEPTNAME")]
        public String ModifyDeptName { get; set; }

        [Column("MODIFYDEPTID")]
        public String ModifyDeptId { get; set; }

        [Column("GroupName")]
        public String GroupName { get; set; }

        [Column("GroupId")]
        public String GroupId { get; set; }
        [Column("GroupCode")]
        public String GroupCode { get; set; }
        [Column("TrainType")]
        public String TrainType { get; set; }
        [Column("TrainTypeName")]
        public String TrainTypeName { get; set; }
        [Column("TrainDateYear")]
        public String TrainDateYear { get; set; }
        [Column("TrainDateMonth")]
        public String TrainDateMonth { get; set; }
        [Column("TrainTarget")]
        public String TrainTarget { get; set; }
        [Column("TrainUserId")]
        public String TrainUserId { get; set; }
        [Column("TrainUserName")]
        public String TrainUserName { get; set; }
        [Column("TrainHostUserId")]
        public String TrainHostUserId { get; set; }
        [Column("TrainHostUserName")]
        public String TrainHostUserName { get; set; }
        [Column("TrainProject")]
        public String TrainProject { get; set; }
        [Column("TrainContent")]
        public String TrainContent { get; set; }
        [Column("Remark")]
        public String Remark { get; set; }
        [Column("VerifyState")]
        public String VerifyState { get; set; }
        [Column("SubmitDate")]
        public DateTime SubmitDate { get; set; }
        [Column("SubmitState")]
        public String SubmitState { get; set; }
        [Column("workState")]
        public String workState { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files1 { get; set; }
    }
}
