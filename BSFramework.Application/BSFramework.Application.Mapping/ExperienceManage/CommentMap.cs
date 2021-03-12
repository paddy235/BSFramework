using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ExperienceManage;

namespace BSFramework.Application.Mapping.ExperienceManage
{
    public class CommentMap : EntityTypeConfiguration<CommentEntity>
    {
        public CommentMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_COMMENT");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
