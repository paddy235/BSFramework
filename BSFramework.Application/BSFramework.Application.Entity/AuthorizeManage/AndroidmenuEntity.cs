using BSFramework.Application.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.BaseManage
{
    [Table("wg_androidmenu")]
    public class AndroidmenuEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 
        /// </summary>
        [Column("menuid")]
        public string MenuId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Column("menuname")]
        public string MenuName { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        [Column("iseffective")]
        public bool IsEffective { get; set; }

        /// <summary>
        /// 父id
        /// </summary>
        [Column("parentid")]
        public string ParentId { get; set; }
        /// <summary>
        /// 父id
        /// </summary>
        [Column("pmenuname")]
        public string PMenuName { get; set; }
        
        /// <summary>
        ///图标
        /// </summary>
        [Column("icon")]
        public string Icon { get; set; }
        /// <summary>
        /// 是否一级
        /// </summary>
        [Column("ismenu")]
        public bool IsMenu { get; set; }
        /// <summary>
        /// 功能模块
        /// </summary>
        [Column("module")]
        public string Module { get; set; }
        
        /// <summary>
        /// 排序
        /// </summary>
        [Column("sort")]
        public int Sort { get; set; }
        /// <summary>
        ///描述
        /// </summary>
        [Column("description")]
        public string Description { get; set; }
        /// <summary>
        ///创建时间
        /// </summary>
        [Column("createtime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        ///创建人
        /// </summary>
        [Column("createuser")]
        public string CreateUser { get; set; }
        /// <summary>
        ///创建人id
        /// </summary>
        [Column("createuserid")]
        public string CreateUserId { get; set; }

        /// <summary>
        ///修改时间
        /// </summary>
        [Column("modifytime")]
        public DateTime ModifyTime { get; set; }
        /// <summary>
        ///修改人
        /// </summary>
        [Column("modifyuser")]
        public string ModifyUser { get; set; }
        /// <summary>
        ///修改人id
        /// </summary>
        [Column("modifyuserid")]
        public string ModifyUserId { get; set; }


        [NotMapped]

        public List<AndroidmenuEntity> child { get; set; }
        [NotMapped]
        public string worktype { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
          
            if (string.IsNullOrEmpty(this.MenuId))
            {
                this.MenuId = Guid.NewGuid().ToString();
            }
            this.CreateTime = DateTime.Now;
             this.CreateUser= OperatorProvider.Provider.Current().UserName;
            this.CreateUserId= OperatorProvider.Provider.Current().UserId;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.MenuId = keyValue;
            this.ModifyTime = DateTime.Now;
            this.ModifyUser = OperatorProvider.Provider.Current().UserName;
            this.ModifyUserId = OperatorProvider.Provider.Current().UserId;
        }
        #endregion
    }


}
