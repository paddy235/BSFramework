using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.SystemManage
{
    /// <summary>
    /// 描 述：系统日志
    /// </summary>
    public class LogEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 日志主键
        /// </summary>
         [Column("LOGID")]
        public string LogId { get; set; }
        /// <summary>
        /// 分类Id 1-登陆2-访问3-操作4-异常
        /// </summary>
          [Column("CATEGORYID")]
        public int? CategoryId { get; set; }
        /// <summary>
        /// 来源对象主键
        /// </summary>	
       [Column("SOURCEOBJECTID")]
        public string SourceObjectId { get; set; }
        /// <summary>
        /// 来源日志内容
        /// </summary>		
         [Column("SOURCECONTENTJSON")]
        public string SourceContentJson { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>	
          [Column("OPERATETIME")]
        public DateTime? OperateTime { get; set; }
        /// <summary>
        /// 操作用户Id
        /// </summary>
        [Column("OPERATEUSERID")]
        public string OperateUserId { get; set; }
        /// <summary>
        /// 操作用户
        /// </summary>
        [Column("OPERATEACCOUNT")]
        public string OperateAccount { get; set; }
        /// <summary>
        /// 操作类型Id
        /// </summary>	
         [Column("OPERATETYPEID")]
        public string OperateTypeId { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
         [Column("OPERATETYPE")]
        public string OperateType { get; set; }
        /// <summary>
        /// 系统功能主键
        /// </summary>	
         [Column("MODULEID")]
        public string ModuleId { get; set; }
        /// <summary>
        /// 系统功能
        /// </summary>		
        [Column("MODULE")]
        public string Module { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>	
        [Column("IPADDRESS")]
        public string IPAddress { get; set; }
        /// <summary>
        /// IP地址所在城市
        /// </summary>
        [Column("IPADDRESSNAME")]
        public string IPAddressName { get; set; }
        /// <summary>
        /// 主机
        /// </summary>	
        [Column("HOST")]
        public string Host { get; set; }
        /// <summary>
        /// 浏览器
        /// </summary>	
          [Column("BROWSER")]
        public string Browser { get; set; }
        /// <summary>
        /// 执行结果状态
        /// </summary>	
         [Column("EXECUTERESULT")]
        public int? ExecuteResult { get; set; }
        /// <summary>
        /// 执行结果信息
        /// </summary>	
        [Column("EXECUTERESULTJSON")]
        public string ExecuteResultJson { get; set; }
        /// <summary>
        /// 备注
        /// </summary>	
         [Column("DESCRIPTION")]
        public string Description { get; set; }
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
        #endregion
    }
}
