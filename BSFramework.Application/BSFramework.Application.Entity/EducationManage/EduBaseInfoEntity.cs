using BSFramework.Application.Entity.Activity;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.EducationManage
{
    [Table("WG_EDUBASEINFO")]
    public class EduBaseInfoEntity
    {
        public EduBaseInfoEntity()
        {
            this.Answers = new List<EduAnswerEntity>();
        }
        [Column("ID")]
        public String ID { get; set; }

        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }

        [Column("STARTDATE")]
        public DateTime StartDate { get; set; }

        [Column("CREATEUSER")]
        public String CreateUser { get; set; }

        [Column("ANSWERCONTENT")]
        public String AnswerContent { get; set; }

        [Column("BZID")]
        public String BZId { get; set; }

        [Column("BZNAME")]
        public String BZName { get; set; }

        [Column("DEPTID")]
        public String DeptId { get; set; }
        /// <summary>
        /// 活动时间
        /// </summary>
        [Column("ACTIVITYDATE")]
        public DateTime? ActivityDate { get; set; }

        /// <summary>
        /// 活动时长
        /// </summary>
        [Column("ACTIVITYTIME")]
        public String ActivityTime { get; set; }
        /// <summary>
        /// 活动地点
        /// </summary>
        [Column("ACTIVITYLOCATION")]
        public String ActivityLocation { get; set; }
        /// <summary>
        /// 题目id
        /// </summary>
        [Column("INVENTORYID")]
        public String InventoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("TEACHERID")]
        public String TeacherId { get; set; }
        /// <summary>
        /// 主讲人 / 新技术讲课，技术问答 出题人
        /// </summary>
        [Column("TEACHER")]
        public String Teacher { get; set; }
        /// <summary>
        /// 记录人 / 新技术讲课，技术问答 答题人
        /// </summary>
        [Column("REGISTERPEOPLE")]
        public String RegisterPeople { get; set; }
        /// <summary>
        /// 记录人id   新技术讲课，技术问答 答题人id
        /// </summary>
        [Column("REGISTERPEOPLEID")]
        public String RegisterPeopleId { get; set; }
        /// <summary>
        /// 提醒
        /// </summary>
        [Column("REMIND")]
        public String Remind { get; set; }
        /// <summary>
        /// 题目
        /// </summary>
        [Column("THEME")]
        public String Theme { get; set; }
        /// <summary>
        /// 运行方式
        /// </summary>
        [Column("RUNWAY")]
        public String RunWay { get; set; }
        /// <summary>
        /// 参加人员
        /// </summary>
        [Column("ATTENDPEOPLE")]
        public String AttendPeople { get; set; }
        /// <summary>
        /// 参加人员id
        /// </summary>
        [Column("ATTENDPEOPLEID")]
        public String AttendPeopleId { get; set; }
        /// <summary>
        /// 参加人员数量
        /// </summary>
        [Column("ATTENDNUMBER")]
        public int AttendNumber { get; set; }
        /// <summary>
        /// 状态   0 待评价 ； 1 已评价
        /// </summary>
        [Column("APPRAISEFLOW")]
        public String AppraiseFlow { get; set; }
        /// <summary>
        /// 状态   0 待回答 ； 1 已回答； 2 已评价
        /// </summary>
        [Column("ANSWERFLOW")]
        public String AnswerFlow { get; set; }
        /// <summary>
        /// 状态   2：准备中   0：进行中  1：已结束
        /// </summary>
        [Column("FLOW")]
        public String Flow { get; set; }
        /// <summary>
        /// 授课人
        /// </summary>
        [Column("IMPARTPEOPLE")]
        public String ImpartPeople { get; set; }
        /// <summary>
        /// 授课人id
        /// </summary>
        [Column("IMPARTPEOPLEID")]
        public String ImpartPeopleId { get; set; }

        /// <summary>
        /// 签到人员
        /// </summary>
        [Column("SIGNPEOPLE")]
        public String SignPeople { get; set; }
        /// <summary>
        /// 签到人员id
        /// </summary>
        [Column("SIGNPEOPLEID")]
        public String SignPeopleId { get; set; }
        /// <summary>
        /// 签到数量
        /// </summary>
        [Column("SIGNNUMBER")]
        public int SignNumber { get; set; }

        [Column("DEFAULTNUMBER")]
        public int DefaultNumber { get; set; }

        [Column("LEARNTIME")]
        public decimal LearnTime { get; set; }
        /// <summary>
        /// 缺席人员
        /// </summary>
        [Column("DEFAULTPEOPLE")]
        public String DefaultPeople { get; set; }
        /// <summary>
        /// 缺席人员id
        /// </summary>
        [Column("DEFAULTPEOPLEID")]
        public String DefaultPeopleId { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [Column("DESCRIBE")]
        public String Describe { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        [Column("REGISTERDATE")]
        public DateTime? RegisterDate { get; set; }
        /// <summary>
        /// 是否保存  y/n
        /// </summary>
        [Column("ISSAVED")]
        public String IsSaved { get; set; }
        /// <summary>
        /// 教育培训类型  1.技术讲课   2.技术问答  3.事故预想 4.反事故预想 5.新技术问答  6.新事故预想  7.拷问讲解 8考问讲解（集中式）
        /// </summary>
        [Column("EDUTYPE")]
        public String EduType { get; set; }

        /// <summary>
        /// 效果评价
        /// </summary>
        [Column("APPRAISECONTENT")]
        public String AppraiseContent { get; set; }

        [Column("NEWAPPRAISECONTENT")]
        public String NewAppraiseContent { get; set; }
        /// <summary>
        /// 评分
        /// </summary>
        [Column("GRADE")]
        public String Grade { get; set; }
        /// <summary>
        /// 评价人
        /// </summary>
        [Column("APPRAISEPEOPLE")]
        public String AppraisePeople { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("APPRAISEPEOPLEID")]
        public String AppraisePeopleId { get; set; }

        /// <summary>
        /// 评分时间
        /// </summary>
        [Column("APPRAISEDATE")]
        public DateTime? AppraiseDate { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [Column("ACTIVITYENDDATE")]
        public DateTime? ActivityEndDate { get; set; }

        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files1 { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files2 { get; set; }

        [NotMapped]
        public IList<ActivityEvaluateEntity> Appraises { get; set; }

        [NotMapped]
        public string BgImage { get; set; }

        [NotMapped]
        public bool? hasSign { get; set; }

        [NotMapped]
        public IList<EduAnswerEntity> Answers { get; set; }
        [Column("CATEGORY")]
        public string Category { get; set; }
        [NotMapped]
        public string Status { get; set; }
        [Column("MEETINGID")]
        public string MeetingId { get; set; }
    }

    public class LeartCount
    {
        public string dept { get; set; }
        public int nums { get; set; }
        public decimal hours { get; set; }
        public decimal avghours { get; set; }
    }
    public class NewEntity
    {
        public string ID { get; set; }
        public string Type { get; set; }
        public string Theme { get; set; }
        public string BZName { get; set; }
        public string BZID { get; set; }
        public DateTime Date { get; set; }

        public string Remark { get; set; }
    }

    // 教育培训类型  1.技术讲课   2.技术问答  3.事故预想 4.反事故演习 5.新技术问答  6.新事故预想  7.拷问讲解
    /// <summary>
    /// 
    /// </summary>
    public enum EduTypeEnum
    {
        [Description("技术讲课")]
        jsjk = 1,
        [Description("技术问答")]
        jswd = 2,
        [Description("事故预想")]
        sgyx = 3,
        [Description("反事故演习")]
        fsgyx = 4,
        [Description("技术讲课")]
        xjswd = 5,
        [Description("事故预想")]
        xsgyx = 6,
        [Description("拷问讲解")]
        kwjj = 7
    }
}
