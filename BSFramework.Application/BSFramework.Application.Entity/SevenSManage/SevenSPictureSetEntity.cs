using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SevenSManage
{
    [Table("wg_sevenspictureset")]
    public   class SevenSPictureSetEntity : BaseEntity
    {
        [Column("Id")]
        public String Id { get; set; }
        [Column("space")]
        public String space { get; set; }
        [Column("remark")]
        public String remark { get; set; }
        [Column("createtime")]
        public String createtime { get; set; }
        [NotMapped]
        public String PictureId { get; set; }
        [NotMapped]
        public List<FileInfoEntity> Files { get; set; }
        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            if (string.IsNullOrEmpty(this.Id))
            {
                this.Id = new Guid().ToString();
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
