using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.ClutureWallManage
{
    public class CultureWallInfoEntity
    {
        public Guid wallinfoid { get; set; }
        public string departmentid { get; set; }
        public string departmentname { get; set; }
        public string summary { get; set; }
        public DateTime? summarydate { get; set; }
        public string slogan { get; set; }
        public DateTime? slogandate { get; set; }
        public string vision { get; set; }
        public DateTime? visiondate { get; set; }
        public string concept { get; set; }
        public DateTime? conceptdate { get; set; }
        public DateTime? createtime { get; set; }
        public string createuserid { get; set; }
        public string savetype { get; set; }
        public string bzid { get; set; }
        public List<CultureWallInfoPicsEntity> pics { get; set; }

    }

    public class CultureWallInfoPicsEntity
    {
        public Guid fileid { get; set; }
        public string filepath { get; set; }
        public string filename { get; set; }
        public int? filetype { get; set; }
        public string description { get; set; }
        public DateTime? createtime { get; set; }
    }
}
