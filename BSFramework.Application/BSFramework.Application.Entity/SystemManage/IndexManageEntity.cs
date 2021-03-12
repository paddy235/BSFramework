using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 首页配置表
    /// </summary>
    [Table("BASE_INDEXMANAGE")]
   public class IndexManageEntity : BaseEntity
    {
        [Column("ID")]
        public string Id { get; set; }
        [Column("DEPTID")]
        public string DeptId { get; set; }
        [Column("TITLE")]
        /// <summary>
        /// 配置的标题
        /// </summary>
        public string Title { get; set; }
        [Column("DEPTCODE")]
        public string DeptCode { get; set; }
        [Column("DEPTNAME")]
        public string DeptName { get; set; }
        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }
        [Column("CREATEDATE")]
        public DateTime CreateDate { get; set; }
        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }
        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }
        [Column("MODIFYDATE")]
        public DateTime ModifyDate { get; set; }
        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }
        [Column("SORT")]
        public int? Sort { get; set; }
        [Column("ISSHOW")]
        public int? IsShow { get; set; }
        /// <summary>
        /// 分类类型 0平台端  1 安卓终端 2手机APP    
        /// </summary>
        [Column("INDEXTYPE")]
        public int? IndexType { get; set; }

        /// <summary>
        /// 所属的模板  1第一套  2 第二套 一次类推
        /// </summary>
        [Column("TEMPLET")]
        public int? Templet { get; set; }
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Id = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
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
            this.Id = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }

    /// <summary>
    /// 指标所属平台
    /// </summary>
    public enum IndexType
    {
        平台端, 
        安卓终端, 
        手机APP
    }

    /// <summary>
    /// 所属模板
    /// </summary>
    public enum Templet
    {
        第一套 = 1, 
        第二套 = 2, 
        第三套 = 3
    }
}
