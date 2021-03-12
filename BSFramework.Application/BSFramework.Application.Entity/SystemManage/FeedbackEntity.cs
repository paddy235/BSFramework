using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：意见反馈
    /// </summary>
    public class FeedbackEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 意见ID
        /// </summary>	
        [Column("OPINIONID")]
        public string OpinionId { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>		
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>	
        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>		
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 更新人
        /// </summary>	
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>	
        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 更新人名称
        /// </summary>	
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        /// <summary>
        /// 创建人部门code 
        /// </summary>	
        [Column("CREATEUSERDEPTCODE")]
        public string CreateUserDeptCode { get; set; }
        /// <summary>
        /// 创建人组织code
        /// </summary>	
        [Column("CREATEUSERORGCODE")]
        public string CreateUserOrgCode { get; set; }
        /// <summary>
        /// 意见反馈人姓名
        /// </summary>	
        [Column("OPINIONPERSONNAME")]
        public string OpinionPersonName { get; set; }
        /// <summary>
        /// 意见反馈人ID
        /// </summary>	
        [Column("OPINIONPERSONID")]
        public string OpinionPersonID { get; set; }
        /// <summary>
        /// 意见反馈部门名称
        /// </summary>	
        [Column("OPINIONDEPTNAME")]
        public string OpinionDeptName { get; set; }
        /// <summary>
        /// 意见反馈部门ID
        /// </summary>	
        [Column("OPINIONDEPTCODE")]
        public string OpinionDeptCode { get; set; }
        /// <summary>
        /// 意见反馈标题
        /// </summary>	
        [Column("OPINIONTITLE")]
        public string OpinionTitle { get; set; }
        /// <summary>
        /// 意见反馈内容
        /// </summary>	
        [Column("OPINIONCONTENT")]
        public string OpinionContent { get; set; }
        /// <summary>
        /// 意见反馈照片
        /// </summary>	
        [Column("OPINIONPHOTO")]
        public string OpinionPhoto { get; set; }
        /// <summary>
        /// 意见反馈时间
        /// </summary>	
        [Column("OPINIONDATE")]
        public DateTime? OpinionDate { get; set; }
        /// <summary>
        /// 是否处理
        /// </summary>	
        [Column("ISDISPOSE")]
        public string IsDispose { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>	
        [Column("DISPOSEPERSONNAME")]
        public string DisposePersonName { get; set; }
        /// <summary>
        /// 处理人ID
        /// </summary>	
        [Column("DISPOSEPERSONID")]
        public string DisposePersonId { get; set; }
        /// <summary>
        /// 处理部门名称
        /// </summary>	
        [Column("DISPOSEDEPTNAME")]
        public string DisposeDeptName { get; set; }
        /// <summary>
        /// 处理部门ID
        /// </summary>	
        [Column("DISPOSEDEPTCODE")]
        public string DisposeDeptCode { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>	
        [Column("DISPOSEDATE")]
        public DateTime? DisposeDate { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.OpinionId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.CreateUserDeptCode = OperatorProvider.Provider.Current().DeptCode;
            this.CreateUserOrgCode = OperatorProvider.Provider.Current().OrganizeCode;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.OpinionId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
