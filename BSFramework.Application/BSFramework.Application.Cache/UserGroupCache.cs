
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BSFramewrok.Application.Cache
{
    /// <summary>
    /// 描 述：用户组信息缓存
    /// </summary>
    public class UserGroupCache
    {

        ///// <summary>
        ///// 用户组列表
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<RoleEntity> GetList()
        //{
        //    var cacheList = CacheFactory.Cache().GetCache<IEnumerable<RoleEntity>>(busines.cacheKey);
        //    if (cacheList == null)
        //    {
        //        var data = busines.GetList();
        //        CacheFactory.Cache().WriteCache(data, busines.cacheKey);
        //        return data;
        //    }
        //    else
        //    {
        //        return cacheList;
        //    }
        //}

        //    public object GetList(string key)
        //{
        //    CacheFactory.Cache().GetCache<IEnumerable<RoleEntity>>(busines.cacheKey);
        //}

        ///// <summary>
        ///// 用户组列表
        ///// </summary>
        ///// <param name="organizeId">公司Id</param>
        ///// <returns></returns>
        //public IEnumerable<RoleEntity> GetList(string organizeId)
        //{
        //    var data = this.GetList();
        //    if (!string.IsNullOrEmpty(organizeId))
        //    {
        //        data = data.Where(t => t.OrganizeId == organizeId);
        //    }
        //    return data;
        //}
    }
}
