using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BSFramework.Application.Entity;
using BSFramework.Application.Code;

namespace BSFramework.Application.Entity.AuthorizeManage
{
    /// <summary>
    /// 描 述：过滤时段
    /// </summary>
    [Table("BASE_FILTERTIME")]
    public class FilterTimeEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 过滤时段主键
        /// </summary>		
        [Column("FILTERTIMEID")]
        public string FilterTimeId { get; set; }
        /// <summary>
        /// 对象类型
        /// </summary>		
        [Column("OBJECTTYPE")]
        public string ObjectType { get; set; }
        /// <summary>
        /// 对象Id
        /// </summary>		
       [Column("OBJECTID")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 访问
        /// </summary>		
        [Column("VISITTYPE")]
        public int? VisitType { get; set; }
        /// <summary>
        /// 星期一
        /// </summary>		
        [Column("WEEKDAY1")]
        public string WeekDay1 { get; set; }
        /// <summary>
        /// 星期二
        /// </summary>		
       [Column("WEEKDAY2")]
        public string WeekDay2 { get; set; }
        /// <summary>
        /// 星期三
        /// </summary>		
        [Column("WEEKDAY3")]
        public string WeekDay3 { get; set; }
        /// <summary>
        /// 星期四
        /// </summary>		
       [Column("WEEKDAY4")]
        public string WeekDay4 { get; set; }
        /// <summary>
        /// 星期五
        /// </summary>		
       [Column("WEEKDAY5")]
        public string WeekDay5 { get; set; }
        /// <summary>
        /// 星期六
        /// </summary>		
       [Column("WEEKDAY6")]
        public string WeekDay6 { get; set; }
        /// <summary>
        /// 星期日
        /// </summary>		
        [Column("WEEKDAY7")]
        public string WeekDay7 { get; set; }
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
            this.FilterTimeId = Guid.NewGuid().ToString();
            this.CreateDate = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
            this.EnabledMark = 1;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.FilterTimeId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
