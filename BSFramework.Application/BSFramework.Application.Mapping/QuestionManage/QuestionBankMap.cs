using BSFramework.Application.Entity.QuestionManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.QuestionManage
{
    public class QuestionBankMap : EntityTypeConfiguration<QuestionBankEntity>
    {
        public QuestionBankMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_QUESTIONBANK");
            //主键
            this.HasKey(t => t.Id);
            #endregion

            #region 配置关系
            #endregion
        }
    }
}
