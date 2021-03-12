using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BSFramework.Application.Service.WorkSys;
using BSFramework.Application.Entity;
using BSFramework.Application.IService;

namespace BSFramework.Application.Busines
{
  public  class WorksetupBll
    {
        private IWorkSerupService service = new WorkSetupService();
        #region
        /// <summary>
        /// 班制类别
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkSetTypeEntity> GetList()
        {
            return service.GetList();
        }
        #endregion
    }
}
