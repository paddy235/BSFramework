using BSFramework.Application.Entity.AppVersionManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.AppVersionManage
{
   public  class AppVersionMap : EntityTypeConfiguration<AppVersionEntity>
    {
       public AppVersionMap() 
       {
           this.ToTable("WG_APPVERSION");
           //主键
           this.HasKey(t => t.Id);
       }
    }
}
