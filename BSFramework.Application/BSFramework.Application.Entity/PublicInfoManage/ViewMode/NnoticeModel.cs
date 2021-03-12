using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Entity.PublicInfoManage.ViewMode
{
   public class NnoticeModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Publisher { get; set; }
        public string PublisherDept { get; set; }
        public DateTime? ReleaseTime { get; set; }
        public string Content { get; set; }
    }
}
