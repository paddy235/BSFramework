using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;


namespace BSFramework.Application.Mapping.EducationManage
{
    public class EduPlanInfoMap : EntityTypeConfiguration<EduPlanInfoEntity>
    {
        public EduPlanInfoMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_EDUPLANINFO");
            //主键
            this.HasKey(t => t.ID);
            #endregion
        }
    }
}
