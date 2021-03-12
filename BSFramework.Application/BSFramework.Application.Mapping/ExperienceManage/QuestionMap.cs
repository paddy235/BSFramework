using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.ExperienceManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.ExperienceManage
{
    public class QuestionMap : EntityTypeConfiguration<QuestionEntity>
    {
        public QuestionMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_QUESTION");
            //主键
            this.HasKey(t => t.ID);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
