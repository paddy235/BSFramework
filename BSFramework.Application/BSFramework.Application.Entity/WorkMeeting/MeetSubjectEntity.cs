using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using System.Collections;
using System.Collections.Generic;
using BSFramework.Application.Entity.PublicInfoManage;

namespace BSFramework.Application.Entity.WorkMeeting
{
    /// <summary>
    /// 班前一课
    /// </summary>
    [Table("WG_MEETSUBJECT")]
    public class MeetSubjectEntity : BaseEntity
    {
        /// <summary>
        ///主键id
        /// </summary>
        [Column("ID")]
        public string ID { get; set; }

        /// <summary>
        ///内容
        /// </summary>
        [Column("CONTENT")]
        public string Content { get; set; }

        /// <summary>
        ///评分
        /// </summary>
        [Column("SCORE")]
        public decimal Score { get; set; }
        /// <summary>
        ///状态
        /// </summary>
        [Column("STATE")]
        public bool State { get; set; }
        /// <summary>
        ///讲课人
        /// </summary>
        [Column("TEACHUSER")]
        public string TeachUser { get; set; }
        /// <summary>
        ///讲课人
        /// </summary>
        [Column("TEACHUSERID")]
        public string TeachUserId { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>		
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>		
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
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
        /// <summary>
        /// 活动id
        /// </summary>		
        [Column("MEETID")]
        public string MeetId { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>		
        [Column("DEPTID")]
        public string Deptid { get; set; }
        /// <summary>
        /// 部门code
        /// </summary>		
        [Column("DEPTCODE")]
        public string Deptcode { get; set; }
        /// <summary>
        /// 部门
        /// </summary>		
        [Column("DEPTNAME")]
        public string Deptname { get; set; }
        /// <summary>
        /// 文件
        /// </summary>
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }

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
