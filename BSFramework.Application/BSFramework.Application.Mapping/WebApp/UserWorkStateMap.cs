using BSFramework.Application.Entity.WebApp;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.WebApp
{
    public class UserWorkStateMap : EntityTypeConfiguration<UserWorkStateEntity>
    {
        public UserWorkStateMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_USERWORKSTATE");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}