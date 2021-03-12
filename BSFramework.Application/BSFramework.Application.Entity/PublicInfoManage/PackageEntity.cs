using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// �� ����app�汾
    /// </summary>
    [Table("WG_PACKAGE")]
    public class PackageEntity : BaseEntity
    {
        #region ʵ���Ա
        /// <summary>
        /// ����ID
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
        /// ������
        /// </summary>
        [Column("PACKTYPE")]
        public int PackType { get; set; }
        /// <summary>
        /// ����汾
        /// </summary>
        /// <returns></returns>
        [Column("RELEASEVERSION")]
        public string ReleaseVersion { get; set; }
        /// <summary>
        /// �����汾
        /// </summary>
        /// <returns></returns>
        [Column("PUBLISHVERSION")]
        public string PublishVersion { get; set; }
        /// <summary>
        /// Ӧ�ó�����
        /// </summary>
        /// <returns></returns>
        [Column("APPNAME")]
        public string AppName { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [Column("RELEASEDATE")]
        public DateTime? ReleaseDate { get; set; }
        /// <summary>
        /// �ļ���
        /// </summary>
        /// <returns></returns>
        [Column("FILENAME")]
        public string FileName { get; set; }


        /// <summary>
        /// APK����
        /// </summary>
        [Column("APKTYPE")]
        public string ApkType { get; set; }



        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
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
        /// �༭����
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