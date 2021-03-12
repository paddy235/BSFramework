using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;

namespace BSFramework.Application.Mapping.EducationManage
{
    public class EduCommentTagMap : EntityTypeConfiguration<EduCommentTagEntity>
    {
        public EduCommentTagMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_EDUCOMMENTTAG");
            //主键
            this.HasKey(t => t.ID);
            #endregion
        }
    }
}
