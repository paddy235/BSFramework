using BSFramework.Application.Entity.WorkMeeting;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.WorkMeeting
{
    public class DangerAnalysisMap : EntityTypeConfiguration<DangerAnalysisEntity>
    {
        public DangerAnalysisMap()
        {
            this.ToTable("WG_DANGERANALYSIS");
            this.HasKey(x => x.AnalysisId);
        }
    }
}
