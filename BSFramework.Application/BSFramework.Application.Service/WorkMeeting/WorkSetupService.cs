using BSFramework.Application.Entity;
using BSFramework.Data;
using BSFramework.Data.Repository;
using BSFramework.Util;
using BSFramework.Util.Extension;
using System;
using System.Data.Common;
using System.Linq;
using System.Collections.Generic;
using BSFramework.Application.IService;

namespace BSFramework.Application.Service.WorkSys
{
    /// <summary>
    ///  描 述：班次管理
    /// </summary>
    public class WorkSetupService : RepositoryFactory<WorkSetTypeEntity>, IWorkSerupService
    {
        /// <summary>
        /// 获取班制类别
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WorkSetTypeEntity> GetList()
        {

           return  this.BaseRepository().IQueryable().OrderBy(t => t.SystemType).ToList();
        
        }
    }
}
