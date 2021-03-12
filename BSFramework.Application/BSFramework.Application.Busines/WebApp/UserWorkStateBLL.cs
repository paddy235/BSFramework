using BSFramework.Application.Entity.WebApp;
using BSFramework.Application.IService.WebApp;
using BSFramework.Application.Service.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.Busines.WebApp
{
    public class UserWorkStateBLL
    {

        private IUserWorkStateService service = new UserWorkStateService();

        /// <summary>
        ///人员状态
        /// </summary>
        public void addState(UserWorkStateEntity entity)
        {

            service.addState(entity);
        }


        /// <summary>
        ///获取人员状态
        /// </summary>
        public UserWorkStateEntity selectState(string userid)
        {
            return service.selectState(userid);
        }
    }
}
