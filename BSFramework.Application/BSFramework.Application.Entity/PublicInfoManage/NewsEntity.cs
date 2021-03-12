using BSFramework.Application.Code;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;

namespace BSFramework.Application.Entity.PublicInfoManage
{
    /// <summary>
    /// 描 述：新闻中心
    /// </summary>
    public class NewsEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 新闻主键
        /// </summary>	
        [Column("NEWSID")]
        public string NewsId { get; set; }
        /// <summary>
        /// 类型（1-新闻2-公告）
        /// </summary>		
         [Column("TYPEID")]
        public int? TypeId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>	
             [Column("PARENTID")]
        public string ParentId { get; set; }
        /// <summary>
        /// 所属类别
        /// </summary>		
         [Column("CATEGORY")]
        public string Category { get; set; }
        /// <summary>
        /// 完整标题
        /// </summary>	
         [Column("FULLHEAD")]
        public string FullHead { get; set; }
        /// <summary>
        /// 标题颜色
        /// </summary>	
        [Column("FULLHEADCOLOR")]
        public string FullHeadColor { get; set; }
        /// <summary>
        /// 简略标题
        /// </summary>		
        [Column("BRIEFHEAD")]
        public string BriefHead { get; set; }
        /// <summary>
        /// 作者
        /// </summary>		
          [Column("AUTHORNAME")]
        public string AuthorName { get; set; }
        /// <summary>
        /// 编辑
        /// </summary>		
         [Column("COMPILENAME")]
        public string CompileName { get; set; }
        /// <summary>
        /// Tag词
        /// </summary>		
         [Column("TAGWORD")]
        public string TagWord { get; set; }
        /// <summary>
        /// 关键字
        /// </summary>		
        [Column("KEYWORD")]
        public string Keyword { get; set; }
        /// <summary>
        /// 来源
        /// </summary>	
         [Column("SOURCENAME")]
        public string SourceName { get; set; }
        /// <summary>
        /// 来源地址
        /// </summary>		
          [Column("SOURCEADDRESS")]
        public string SourceAddress { get; set; }
        /// <summary>
        /// 新闻内容
        /// </summary>	
        [Column("NEWSCONTENT")]
        public string NewsContent { get; set; }
        /// <summary>
        /// 浏览量
        /// </summary>		
          [Column("PV")]
        public int? PV { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>	
          [Column("RELEASETIME")]
        public DateTime? ReleaseTime { get; set; }
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
        [Column("DeptId")]
        public string DeptId { get; set; }
        [Column("DeptName")]
        public string DeptName { get; set; }
        [Column("isimport")]
        public bool IsImport { get; set; }
        [Column("noticedept")]
        public string NoticeDept { get; set; }

        [NotMapped]

        public IList fileList { get; set; }

        [NotMapped]
        public string Title { get; set; }

        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CreateDate = DateTime.Now;
            this.ReleaseTime = DateTime.Now;
            this.CreateUserId = OperatorProvider.Provider.Current().UserId;
            this.CreateUserName = OperatorProvider.Provider.Current().UserName;
            this.DeleteMark = 0;
            this.EnabledMark = 1;
            this.PV = 0;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.NewsId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
            this.ModifyUserName = OperatorProvider.Provider.Current().UserName;
        }
        #endregion
    }
}
