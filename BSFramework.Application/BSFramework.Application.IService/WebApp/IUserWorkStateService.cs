using BSFramework.Application.Entity.WebApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSFramework.Application.IService.WebApp
{
 public   interface IUserWorkStateService
    {
     


        /// <summary>
        ///新增人员状态
        /// </summary>
        void addState(UserWorkStateEntity entity);


        /// <summary>
        ///获取人员状态
        /// </summary>
        UserWorkStateEntity selectState(string userid);


    }
}
