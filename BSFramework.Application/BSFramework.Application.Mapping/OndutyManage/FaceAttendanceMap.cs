using BSFramework.Application.Entity.OndutyManage;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Mapping.OndutyManage
{
   public class FaceAttendanceMap : EntityTypeConfiguration<FaceAttendanceEntity>
    {
        public FaceAttendanceMap()
        {
            #region 表、主键
            //表
            this.ToTable("WG_FACEATTENDANCE");
            //主键
            this.HasKey(t => t.id);
            #endregion

            #region 配置关系

            #endregion
        }
    }
}
