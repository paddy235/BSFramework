using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 日 期：2016.04.14 09:16
    /// 描 述：系统表单
    /// </summary>
       [Table("BASE_MODULEFORM")]
    public class ModuleFormEntity:BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 表单主键
        /// </summary>
        [Column("FORMID")]
           public string FormId { set; get; }
        /// <summary>
        /// 功能主键
        /// </summary>
         [Column("MODULEID")]
           public string ModuleId { set; get; }
        /// <summary>
        /// 编码
        /// </summary>
         [Column("ENCODE")]
           public string EnCode { set; get; }
        /// <summary>
        /// 名称
        /// </summary>
         [Column("FULLNAME")]
           public string FullName { set; get; }
        /// <summary>
        /// 表单控件Json
        /// </summary>
          [Column("FORMJSON")]
           public string FormJson { set; get; }
        /// <summary>
        /// 排序码
        /// </summary>
         [Column("SORTCODE")]
           public int? SortCode { set; get; }
        /// <summary>
        /// 删除标记
        /// </summary>
          [Column("DELETEMARK")]
           public int? DeleteMark { set; get; }
        /// <summary>
        /// 有效标志
        /// </summary>
          [Column("ENABLEDMARK")]
           public int? EnabledMark { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
          [Column("DESCRIPTION")]
           public string Description { set; get; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Column("CREATEDATE")]
           public DateTime? CreateDate { set; get; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
          [Column("CREATEUSERID")]
           public string CreateUserId { set; get; }
        /// <summary>
        /// 创建用户
        /// </summary>
          [Column("CREATEUSERNAME")]
           public string CreateUserName { set; get; }
        /// <summary>
        /// 修改日期
        /// </summary>
          [Column("MODIFYDATE")]
           public DateTime? ModifyDate { set; get; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
         [Column("MODIFYUSERID")]
           public string ModifyUserId { set; get; }
        /// <summary>
        /// 修改用户
        /// </summary>
         [Column("MODIFYUSERNAME")]
           public string ModifyUserName { set; get; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.FormId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.FormId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
