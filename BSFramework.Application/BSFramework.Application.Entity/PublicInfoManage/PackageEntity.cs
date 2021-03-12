using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：app版本
    /// </summary>
    [Table("WG_PACKAGE")]
    public class PackageEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键ID
        /// </summary>
        /// <returns></returns>
        [Column("ID")]
        public string ID { get; set; }


        [Column("CREATEDATE")]
        public DateTime? CreateDate { get; set; }


        [Column("CREATEUSERID")]
        public string CreateUserId { get; set; }


        [Column("CREATEUSERNAME")]
        public string CreateUserName { get; set; }

        [Column("CREATEDEPTID")]
        public string CreateDeptId { get; set; }

        [Column("CREATEDEPTCODE")]
        public string CreateDeptCode { get; set; }

        [Column("CREATEDEPTNAME")]
        public string CreateDeptName { get; set; }

        [Column("MODIFYUSERID")]
        public string ModifyUserId { get; set; }

        [Column("MODIFYUSERNAME")]
        public string ModifyUserName { get; set; }

        [Column("MODIFYDATE")]
        public DateTime? ModifyDate { get; set; }

        /// <summary>
        /// 包类型
        /// </summary>
        [Column("PACKTYPE")]
        public int PackType { get; set; }
        /// <summary>
        /// 编译版本
        /// </summary>
        /// <returns></returns>
        [Column("RELEASEVERSION")]
        public string ReleaseVersion { get; set; }
        /// <summary>
        /// 发布版本
        /// </summary>
        /// <returns></returns>
        [Column("PUBLISHVERSION")]
        public string PublishVersion { get; set; }
        /// <summary>
        /// 应用程序名
        /// </summary>
        /// <returns></returns>
        [Column("APPNAME")]
        public string AppName { get; set; }
        /// <summary>
        /// 编译日期
        /// </summary>
        /// <returns></returns>
        [Column("RELEASEDATE")]
        public DateTime? ReleaseDate { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        /// <returns></returns>
        [Column("FILENAME")]
        public string FileName { get; set; }


        /// <summary>
        /// APK类型
        /// </summary>
        [Column("APKTYPE")]
        public string ApkType { get; set; }



        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ID = string.IsNullOrEmpty(ID) ? Guid.NewGuid().ToString() : ID;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.CreateDeptId = OperatorProvider.Provider.Current().DeptId;
            this.CreateDeptCode = OperatorProvider.Provider.Current().DeptCode;
            this.CreateDeptName = OperatorProvider.Provider.Current().DeptName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ID = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}