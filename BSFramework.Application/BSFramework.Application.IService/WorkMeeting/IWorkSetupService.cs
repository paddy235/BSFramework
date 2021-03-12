
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;
using System.Data;
using BSFramework.Application.Entity;
 

namespace BSFramework.Application.IService
{
    /// <summary>
    /// 班制类别
    /// </summary>
    public interface IWorkSerupService
    {
        /// <summary>
        /// 获取班制类别
        /// </summary>
        /// <returns></returns>
        IEnumerable<WorkSetTypeEntity> GetList();
        
    }
}
