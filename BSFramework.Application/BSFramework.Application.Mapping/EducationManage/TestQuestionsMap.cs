using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Entity.EducationManage;
using System.Data.Entity.ModelConfiguration;

namespace BSFramework.Application.Mapping.EducationManage
{
    public class TestQuestionsMap : EntityTypeConfiguration<TestQuestionsEntity>
    {
        public TestQuestionsMap() 
        {
            #region 表、主键
            //表
            this.ToTable("WG_TESTQUESTIONS");
            //主键
            this.HasKey(t => t.Questionsid);
            #endregion
        }
    }
}
