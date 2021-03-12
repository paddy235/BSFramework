using BSFramework.Application.Entity.PublicInfoManage;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.AppVersionManage
{
    [Table("wg_appversion")]
    public class AppVersionEntity : BaseEntity
    {
        [Column("Id")]
        public String Id { get; set; }

        [Column("CreateUser")]
        public String CreateUser { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("ReleaseVersion")]
        public String ReleaseVersion { get; set; }

        [Column("PublishVersion")]
        public String PublishVersion { get; set; }

        [Column("AppName")]
        public String AppName { get; set; }

        [Column("Path")]
        public String Path { get; set; }
    }
}
