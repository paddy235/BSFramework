using BSFramework.Application.Entity.WebApp;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.WebApp
{
   public class UserExperiencMap : EntityTypeConfiguration<UserExperiencEntity>
    {


        public UserExperiencMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_USEREXPERIENC");
            //主键
            this.HasKey(t => t.ExperiencId);
            #endregion

            #region 配置关系
            #endregion
        }

    }
}
