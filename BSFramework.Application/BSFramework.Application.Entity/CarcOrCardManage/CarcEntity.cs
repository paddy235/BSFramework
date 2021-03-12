using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.CarcOrCardManage
{
    /// <summary>
    /// 描 述：Carc 手袋卡 表
    /// </summary>
    [Table("WG_CARC")]
    public class CarcEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id 
        /// </summary>	
        [Key]
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>	

        [Column("WORKNAME")]
        public string WorkName { get; set; }
        /// <summary>
        /// 任务区域
        /// </summary>	

        [Column("WORKAREA")]
        public string WorkArea { get; set; }
        /// <summary>
        /// 任务类别
        /// </summary>	

        [Column("WORKTYPE")]
        public string WorkType { get; set; }
        /// <summary>
        ///  数据类型  card  carc
        /// </summary>	

        [Column("DATATYPE")]
        public string DataType { get; set; }
        /// <summary>
        /// 计划开始时间
        /// </summary>
        [Column("STARTTIME")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        ///计划结束时间
        /// </summary>	
        [Column("ENDTIME")]
        public DateTime? EndTime { get; set; }
        /// <summary>
        ///危险作业许可
        /// </summary>
        [Column("DANGERWORK")]
        public string DangerWork { get; set; }
        /// <summary>
        ///上锁挂牌
        /// </summary>
        [Column("LOCKEDUP")]
        public string LockedUp { get; set; }

        /// <summary>
        ///安全站位
        /// </summary>
        [Column("SAFELOCATION")]
        public string SafeLocation { get; set; }
        /// <summary>
        ///安全沟通
        /// </summary>
        [Column("SAFECOMMUNICATION")]
        public string SafeCommunication { get; set; }
        /// <summary>
        ///确定的操作程序
        /// </summary>
        [Column("OPERATIONPROGRAM")]
        public string OperationProgram { get; set; }
        /// <summary>
        ///监护人员 责任人
        /// </summary>
        [Column("TUTELAGEPERSON")]
        public string TutelagePerson { get; set; }
        /// <summary>
        ///监护人员id
        /// </summary>
        [Column("TUTELAGEPERSONID")]
        public string TutelagePersonId { get; set; }
        /// <summary>
        ///操作人员 工作成员
        /// </summary>
        [Column("OPERATIONPERSON")]
        public string OperationPerson { get; set; }
        /// <summary>
        ///操作人员id
        /// </summary>
        [Column("OPERATIONPERSONID")]
        public string OperationPersonId { get; set; }


        /// <summary>
        ///主要操作步骤
        /// </summary>
        [Column("MAINOPERATION")]
        public string MainOperation { get; set; }
        /// <summary>
        ///班会id
        /// </summary>
        [Column("MEETID")]
        public string MeetId { get; set; }
        /// <summary>
        ///状态  0 保存 1 结束
        /// </summary>
        [Column("STATE")]
        public int State { get; set; }
        /// <summary>
        ///组织id
        /// </summary>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        ///组织code
        /// </summary>
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        ///组织名称
        /// </summary>
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        ///创建人时间
        /// </summary>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        ///创建人id
        /// </summary>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        ///修改人时间
        /// </summary>
        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }
        /// <summary>
        ///修改人id
        /// </summary>
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        ///修改人
        /// </summary>
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 风险标识
        /// </summary>
        [NotMapped]
        public IList<CDangerousEntity> CDangerousList { get; set; }

        /// <summary>
        /// 文件
        /// </summary>
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [NotMapped]
        public IList<ActivityEvaluateEntity> Evaluates { get; set; }
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
