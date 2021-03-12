using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    public class EduBaseInfoMap : EntityTypeConfiguration<EduBaseInfoEntity>
    {
        public EduBaseInfoMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_EDUBASEINFO");
            //主键
            this.HasKey(t => t.ID);
            #endregion
        }
    }
}
