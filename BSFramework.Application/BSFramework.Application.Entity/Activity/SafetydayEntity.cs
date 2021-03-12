using BSFramework.Application.Code;
using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.Activity
{
    [Table("WG_SAFETYDAY")]
    public class SafetydayEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Column("ID")]
        public string Id { get; set; }
        /// <summary>
        /// 记录创建时间
        /// </summary>
        /// <returns></returns>
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 记录创建人编码
        /// </summary>
        /// <returns></returns>
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 记录创建人姓名
        /// </summary>
        /// <returns></returns>
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 最近一次修改人编码
        /// </summary>
        /// <returns></returns>
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 最近一次修改人姓名
        /// </summary>
        /// <returns></returns>
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 记录创建人所在部门编码
        /// </summary>
        /// <returns></returns>
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        /// <summary>
        /// 记录创建人所在部门编码
        /// </summary>
        /// <returns></returns>
        [Column("CDEPTID")]
        public string CDeptId { get; set; }
        /// <summary>
        /// 记录创建人所在部门编码
        /// </summary>
        /// <returns></returns>
        [Column("CDEPTNAME")]
        public string CDeptName { get; set; }
        /// <summary>
        /// 活动主题
        /// </summary>
        /// <returns></returns>
        [Column("SUBJECT")]
        public string Subject { get; set; }
        /// <summary>
        /// 活动说明
        /// </summary>
        /// <returns></returns>
        [Column("EXPLAIN")]
        public string Explain { get; set; }
        /// <summary>
        /// 选择班组编码，多个用逗号分隔
        /// </summary>
        /// <returns></returns>
        [Column("DEPTID")]
        public string DeptId { get; set; }
        /// <summary>
        /// 选择班组名称，多个用逗号分隔
        /// </summary>
        /// <returns></returns>
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        /// <summary>
        /// 关联班组活动，多个用逗号分隔
        /// </summary>
        /// <returns></returns>
        [Column("ACTIDS")]
        public string ActIds { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [Column("REMARK")]
        public string Remark { get; set; }
        /// <summary>
        /// 活动类型
        /// </summary>
        [Column("ACTIVITYTYPE")]
        public string ActivityType { get; set; }

        [NotMapped]
        public string Material { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [NotMapped]
        public int? state { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files
        {
            get; set;
        }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = string.IsNullOrEmpty(this.Id) ? Guid.NewGuid().ToString() : this.Id;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeptCode = string.IsNullOrEmpty(OperatorProvider.Provider.Current().DeptCode) ? OperatorProvider.Provider.Current().OrganizeCode : OperatorProvider.Provider.Current().DeptCode;
            this.CDeptId = string.IsNullOrEmpty(OperatorProvider.Provider.Current().DeptId) ? OperatorProvider.Provider.Current().OrganizeId : OperatorProvider.Provider.Current().DeptId;
            this.CDeptName = string.IsNullOrEmpty(OperatorProvider.Provider.Current().DeptName) ? OperatorProvider.Provider.Current().OrganizeName : OperatorProvider.Provider.Current().DeptName;

        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.Id = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
