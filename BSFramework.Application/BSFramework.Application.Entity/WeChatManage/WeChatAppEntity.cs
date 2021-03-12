using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.WeChatManage 
{
    /// <summary>
    /// 描 述：企业号应用
    /// </summary>
    [Table("WECHAT_APP")]
    public class WeChatAppEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 应用主键
        /// </summary>		
        [Column("APPID")]
        public string AppId { get; set; }
        /// <summary>
        /// 应用Logo
        /// </summary>		
        [Column("APPLOGO")]
        public string AppLogo { get; set; }
        /// <summary>
        /// 应用名称
        /// </summary>		
        [Column("APPNAME")]
        public string AppName { get; set; }
        /// <summary>
        /// 应用类型
        /// </summary>		
        [Column("APPTYPE")]
        public int? AppType { get; set; }
        /// <summary>
        /// 应用介绍
        /// </summary>		
        [Column("DESCRIPTION")]
        public string Description { get; set; }
        /// <summary>
        /// 应用主页
        /// </summary>		
        [Column("APPURL")]
        public string AppUrl { get; set; }
        /// <summary>
        /// 可信域名
        /// </summary>		
        [Column("REDIRECTDOMAIN")]
        public string RedirectDomain { get; set; }
        /// <summary>
        /// 应用菜单
        /// </summary>		
        [Column("MENUJSON")]
        public string MenuJson { get; set; }
        /// <summary>
        /// 是否接收用户变更通知
        /// </summary>		
        [Column("ISREPORTUSER")]
        public int? IsReportUser { get; set; }
        /// <summary>
        /// 是否上报用户进入应用事件
        /// </summary>		
        [Column("ISREPORTENTER")]
        public int? IsReportenter { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>		
        [Column("DELETEMARK")]
        public int DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>		
        [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.AppId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}