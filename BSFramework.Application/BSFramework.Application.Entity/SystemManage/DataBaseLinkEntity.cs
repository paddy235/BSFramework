using BSFramework.Application.Code;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：数据库连接管理
    /// </summary>
    public class DataBaseLinkEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 数据库连接主键
        /// </summary>	
        [Column("DATABASELINKID")]
        public string DatabaseLinkId { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>	
        [Column("SERVERADDRESS")]
        public string ServerAddress { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>		
         [Column("DBNAME")]
        public string DBName { get; set; }
        /// <summary>
        /// 数据库别名
        /// </summary>	
         [Column("DBALIAS")]
        public string DBAlias { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>		
        [Column("DBTYPE")]
        public string DbType { get; set; }
        /// <summary>
        /// 数据库版本
        /// </summary>
        [Column("VERSION")]
        public string Version { get; set; }
        /// <summary>
        /// 连接地址
        /// </summary>	
       [Column("DBCONNECTION")]
        public string DbConnection { get; set; }
        /// <summary>
        /// 连接地址是否加密
        /// </summary>
        [Column("DESENCRYPT")]
        public int? DESEncrypt { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>	
         [Column("SORTCODE")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>	
        [Column("DELETEMARK")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
         [Column("ENABLEDMARK")]
        public int? EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>	
        [Column("DESCRIPTION")]
        public string Description { get; set; }
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
            this.DatabaseLinkId = Guid.NewGuid().ToString();
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
            this.DatabaseLinkId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}