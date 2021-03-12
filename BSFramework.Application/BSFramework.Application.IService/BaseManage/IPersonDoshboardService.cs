using BSFramework.Application.Entity.BaseManage;
using BSFramework.Util.WebControl;
using System.Collections.Generic;

namespace BSFramework.Application.IService.BaseManage
{
    /// <summary>
    /// 个人首页内容
    /// </summary>
    public interface IPersonDoshboardService
    {
        List<PersonDoshboardEntity> GetList(string userId);
        void Save(List<PersonDoshboardEntity> settings);
        List<PersonDoshboardEntity> GetEnabledList(string userId);
    }
}
