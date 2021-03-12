
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.WebApp
{
    [Table("wg_userworkstate")]
    public class UserWorkStateEntity : BaseEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键id
        /// </summary>	
        [Key]
        [Column("id")]
        public string Id { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>	
        [Column("userid")]
        public string userId { get; set; }
        /// <summary>
        /// 状态
        /// </summary>	
        [Column("state")]
        public string State { get; set; }
        #endregion
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                this.Id = Guid.NewGuid().ToString();
            }
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {

        }
        #endregion
    }
}
