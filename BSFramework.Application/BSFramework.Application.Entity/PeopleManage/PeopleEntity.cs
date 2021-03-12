using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PeopleManage
{
    /// <summary>
    /// 成员管理
    /// </summary>
    [Table("WG_PEOPLE")]
    public class PeopleEntity : BaseEntity
    {

        public PeopleEntity()
        {
            this.Files = new List<FileInfoEntity>();
        }
        #region 实体成员
        /// <summary>
        /// 成员ID
        /// </summary>
        [Column("ID")]
        public String ID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [Column("NAME")]
        public String Name { get; set; }
        /// <summary>
        ///专业类别
        /// </summary>
        [Column("SPECIALTYTYPE")]
        public string SpecialtyType { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        [Column("SEX")]
        public String Sex { get; set; }
        /// <summary>
        /// 职务名称 （多选，逗号隔开）
        /// </summary>
        [Column("QUARTERS")]
        public String Quarters { get; set; }
        /// <summary>
        /// 职务id  
        /// </summary>
        [Column("QUARTERID")]
        public String QuarterId { get; set; }

        [Column("ROLEDUTYNAME")]
        public String RoleDutyName { get; set; }
        [Column("ROLEDUTYID")]
        public String RoleDutyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("AGE")]
        public String Age { get; set; }
        /// <summary>
        /// 政治面貌
        /// </summary>
        [Column("VISAGE")]
        public String Visage { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        [Column("LINKWAY")]
        public String LinkWay { get; set; }
        /// <summary>
        /// 技术等级
        /// </summary>
        [Column("TECLEVEL")]
        public String TecLevel { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [Column("IDENTITYNO")]
        public String IdentityNo { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        [Column("FOLK")]
        public String Folk { get; set; }
        /// <summary>
        /// 学历
        /// </summary>
        [Column("DEGREE")]
        public String Degree { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        [Column("ADDRESS")]
        public String Address { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [Column("LABOURNO")]
        public String LabourNo { get; set; }
        /// <summary>
        /// 所属班组名称
        /// </summary>
        [Column("BZNAME")]
        public String BZName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("BZCODE")]
        public String BZCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Column("BZID")]
        public String BZID { get; set; }
        /// <summary>
        /// 入场时间
        /// </summary>
        [Column("ENTRYDATE")]
        public DateTime EntryDate { get; set; }
        /// <summary>
        /// 健康状况
        /// </summary>
        [Column("HEALTHSTATUS")]
        public String HealthStatus { get; set; }
        /// <summary>
        /// 是否班组成员（在平台修改班组成员为非班组成员，不删除people表数据，修改此字段）
        /// </summary>
        [Column("FINGERMARK")]
        public String FingerMark { get; set; }
        /// <summary>
        /// 电子签名
        /// </summary>
        [Column("ELEIDIOGRAPH")]
        public String EleIdiograph { get; set; }
        [Column("USERACCOUNT")]
        public String UserAccount { get; set; }
        [Column("PASSWORD")]
        public String PassWord { get; set; }
        [Column("REMARK")]
        public String Remark { get; set; }
        [Column("DEPTNAME")]
        public String DeptName { get; set; }
        [Column("DEPTCODE")]
        public String DeptCode { get; set; }



        [Column("PHOTO")]
        public String Photo { get; set; }
        /// <summary>
        /// 职务编号 （排序用 ： 001,002,003）
        /// </summary>
        [Column("PLANER")]
        public String Planer { get; set; }
        [Column("DEPTID")]
        public String DeptId { get; set; }
        [Column("BIRTHDAY")]
        public DateTime? Birthday { get; set; }
        [Column("NATIVE")]
        public String Native { get; set; }
        [Column("OLDDEGREE")]
        public String OldDegree { get; set; }
        [Column("NEWDEGREE")]
        public String NewDegree { get; set; }
        [Column("WORKKIND")]
        public String WorkKind { get; set; }
        [Column("WORKAGE")]
        public String WorkAge { get; set; }
        [Column("CURRENTWORKAGE")]
        public String CurrentWorkAge { get; set; }

        [Column("JOBNAME")]
        public String JobName { get; set; }

        [Column("ISSPECIAL")]
        public String IsSpecial { get; set; }

        [Column("ISSPECIALEQUIPMENT")]
        public String IsSpecialEquipment { get; set; }
        [NotMapped]
        public IList<FileInfoEntity> Files { get; set; }

        [NotMapped]
        public string Percent { get; set; }

        [NotMapped]
        public string Scores { get; set; }
        [NotMapped]
        public string Jobs { get; set; }
        [NotMapped]
        public string state { get; set; }
        [NotMapped]
        public string AllocationId { get; set; }
        [Column("WORKTYPE")]
        public String WorkType { get; set; }

        [NotMapped]
        public string hasface { get; set; }
        [NotMapped]
        public string hasfinger { get; set; }

        [NotMapped]
        public string PostId { get; set; }

        [NotMapped]
        public string PostName { get; set; }

        /// <summary>
        /// 签名
        /// </summary>
        [Column("SIGNATURE")]
        public string Signature { get; set; }

        /// <summary>
        /// 外包单位名称
        /// </summary>
        [Column("EPIBOLYDEPT")]
        public string EpibolyDept { get; set; }
        /// <summary>
        /// 是否外包 1是 其他不是  来自UserEntity
        /// </summary>
        [Column("ISEPIBOLY")]
        public string IsEpiboly { get; set; }

        /// <summary>
        /// 人员活动id
        /// </summary>
        [NotMapped]
        public string ActivityPersonId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 人员类型
        /// </summary>
        [Column("USERTYPE")]
        public string UserType { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Column("TELEPHONE")]
        public string Telephone { get; set; }
        /// <summary>
        /// 角色主键
        /// </summary>		
        [NotMapped]
        public string RoleId { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        /// <summary>
        /// 三种人
        /// </summary>
        [Column("ISFOURPERSON")]
        public string IsFourPerson { get; set; }
        #endregion
    }

    //public class PeopleExtension : PeopleEntity 
    //{
    //    public string imageSrc { get; set; }
    //}


}
