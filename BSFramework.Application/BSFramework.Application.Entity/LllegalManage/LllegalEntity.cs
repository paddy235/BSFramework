using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.LllegalManage
{
    [Table("wg_lllegalregister")]
    public class LllegalEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 违章Id
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 违章编号
        /// </summary>
        [Column("LllegalNumber")]
        public String LllegalNumber { get; set; }
        /// <summary>
        /// 违章类型
        /// </summary>
        [Column("LllegalType")]
        public String LllegalType { get; set; }
        /// <summary>
        /// 违章时间
        /// </summary>
        [Column("LllegalTime")]
        public DateTime LllegalTime { get; set; }
        /// <summary>
        /// 违章等级
        /// </summary>
        [Column("LllegalLevel")]
        public String LllegalLevel { get; set; }
        /// <summary>
        /// 违章人
        /// </summary>
        [Column("LllegalPerson")]
        public String LllegalPerson { get; set; }
        /// <summary>
        /// 违章人Id
        /// </summary>
        [Column("LllegalPersonId")]
        public String LllegalPersonId { get; set; }
        /// <summary>
        /// 违章班组
        /// </summary>
        [Column("LllegalTeam")]
        public String LllegalTeam { get; set; }
        /// <summary>
        /// 违章班组id 
        /// </summary>
        [Column("LllegalTeamId")]
        public String LllegalTeamId { get; set; }
        /// <summary>
        /// 违章部门
        /// </summary>
        [Column("LllegalDepart")]
        public String LllegalDepart { get; set; }
        /// <summary>
        /// 违章部门Id
        /// </summary>
        [Column("LllegalDepartCode")]
        public String LllegalDepartCode { get; set; }
        /// <summary>
        /// 违章描述
        /// </summary>
        [Column("LllegalDescribe")]
        public String LllegalDescribe { get; set; }
        /// <summary>
        /// 登记人
        /// </summary>
        [Column("RegisterPerson")]
        public String RegisterPerson { get; set; }
        /// <summary>
        /// 登记人Id
        /// </summary>
        [Column("RegisterPersonId")]
        public String RegisterPersonId { get; set; }
        /// <summary>
        /// 违章地点
        /// </summary>
        [Column("LllegalAddress")]
        public String LllegalAddress { get; set; }

        [Column("REMARK")]
        public String Remark { get; set; }

        /// <summary>
        /// 核准人
        /// </summary>
        [Column("APPROVEPERSON")]
        public String ApprovePerson { get; set; }

        /// <summary>
        /// 核准时间
        /// </summary>
        [Column("APPROVEDATE")]
        public DateTime? ApproveDate { get; set; }

        /// <summary>
        /// 核准结果 0:核准通过 1：核准不通过
        /// </summary>
        [Column("APPROVERESULT")]
        public String ApproveResult { get; set; }

        /// <summary>
        /// 不予核准原因
        /// </summary>
        [Column("APPROVEREASON")]
        public String ApproveReason { get; set; }


        /// <summary>
        /// 整改人
        /// </summary>
        [Column("REFORMPEOPLE")]
        public String ReformPeople { get; set; }

        /// <summary>
        /// 整改期限
        /// </summary>
        [Column("REFORMDATE")]
        public DateTime? ReformDate { get; set; }

        /// <summary>
        /// 整改人ID
        /// </summary>
        [Column("REFORMPEOPLEID")]
        public String ReformPeopleId { get; set; }

        /// <summary>
        /// 流程状态
        /// </summary>
        [Column("FLOWSTATE")]
        public String FlowState { get; set; }

        /// <summary>
        /// 是否考核
        /// </summary>
        [Column("ISASSESS")]
        public String IsAssess { get; set; }

        /// <summary>
        /// 是否考核
        /// </summary>
        [Column("ASSESSMONEY")]
        public String AssessMoney { get; set; }

        /// <summary>
        /// 提交对象（公司或班组）
        /// </summary>
        [Column("SUB")]
        public String Sub { get; set; }

        /// <summary>
        /// 核准人ID
        /// </summary>
        [Column("APPROVEPERSONID")]
        public String ApprovePersonId { get; set; }

        [Column("reportdept")]
        public string ReportDept { get; set; }
        [Column("checktype")]
        public string CheckType { get; set; }
        [Column("checkcontent")]
        public string CheckContent { get; set; }
        /// <summary>
        /// 整改Id
        /// </summary>
        [Column("RefromId")]
        public string RefromId { get; set; }
        /// <summary>
        /// 整改状态
        /// </summary>
        [Column("RefromState")]
        public string RefromState { get; set; }
        [NotMapped]
        /// <summary>
        /// 图片路劲
        /// </summary>
        public IList fileList { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public LllegalRefromEntity refrom { get; set; }
        [NotMapped]
        public LllegalAcceptEntity accept { get; set; }

        #endregion
    }
}
