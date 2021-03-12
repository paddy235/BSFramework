using BSFramework.Application.Entity.BaseManage;
using BSFramework.Application.IService.BaseManage;
using BSFramework.Application.Service.BaseManage;
using BSFramework.Cache.Factory;
using BSFramework.Util.WebControl;
using System;
using System.Collections.Generic;

namespace BSFramework.Application.Busines.BaseManage
{
    /// <summary>
    /// 个人首页内容
    /// </summary>
    public class PersonDoshboardBLL
    {
        private IPersonDoshboardService doshboardService = new PersonDoshboardService();

        public List<PersonDoshboardEntity> GetEnabledList(string userId)
        {
            return doshboardService.GetEnabledList(userId);
        }

        public List<PersonDoshboardEntity> GetList(string userId)
        {
            return doshboardService.GetList(userId);
        }

        public void Save(List<PersonDoshboardEntity> settings)
        {
            doshboardService.Save(settings);
        }
    }
}
