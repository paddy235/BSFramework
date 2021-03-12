using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.SystemManage
{
    [Table("BASE_INDEXASSOCIATION")]
   public class IndexAssocationEntity : BaseEntity
    {
        public string Id { get; set; }
        public string TitleId { get; set; }
        public string DataSetId { get; set; }
        public string DeptId { get; set; }
    }
}
