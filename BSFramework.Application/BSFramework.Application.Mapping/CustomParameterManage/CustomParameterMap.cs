using BSFramework.Application.Entity.CustomParameterManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.CustomParameterManage
{
   public class CustomParameterMap : EntityTypeConfiguration<CustomParameterEntity>
    {

        public CustomParameterMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_CUSTOMPARAMETER");
            //主键
            this.HasKey(t => t.CPId);
            #endregion
        }
    }
}
